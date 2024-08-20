using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using ReswPlus.Core.ClassGenerator;
using ReswPlus.Core.ResourceInfo;
using ReswPlus.SourceGenerator.Models;
using ReswPlus.SourceGenerator.Pluralization;

namespace ReswPlus.SourceGenerator;

public enum AppType
{
    Unknown,
    MicrosoftResourceLoader,
    WindowsResourceLoader,
    ResourceManager
}

[Generator(LanguageNames.CSharp)]
public partial class ReswSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    /// <summary>
    /// Retrieve the default resource file in the list provided
    /// </summary>
    /// <param name="reswFiles">list of resource files</param>
    /// <param name="defaultLanguage">default language of the project</param>
    /// <returns>the path to the default resource file</returns>
    private string RetrieveDefaultResourceFile(IEnumerable<string> reswFiles, string defaultLanguage)
    {

        var languages = new List<string>();
        if (!string.IsNullOrEmpty(defaultLanguage))
        {
            languages.Add(defaultLanguage);
        }

        if (!"en-us".Equals(defaultLanguage, StringComparison.InvariantCultureIgnoreCase))
        {
            languages.Add("en-us");
        }

        if (!"en".Equals(defaultLanguage, StringComparison.InvariantCultureIgnoreCase))
        {
            languages.Add("en");
        }

        foreach (var language in languages)
        {
            foreach (var reswFile in reswFiles)
            {
                var parentFolderName = Path.GetFileName(Path.GetDirectoryName(reswFile));
                if (parentFolderName.Equals(defaultLanguage, StringComparison.InvariantCultureIgnoreCase))
                {
                    return reswFile;
                }
            }
        }

        // if the default resource file is not found, return the first one.
        return reswFiles.FirstOrDefault();
    }

    public void Execute(GeneratorExecutionContext context)
    {

#if DEBUG
        if (!Debugger.IsAttached)
        {
            // Uncomment to debug
            // Debugger.Launch();
        }
#endif

        if (context.Compilation is not CSharpCompilation csharpCompilation)
        {
            // the converter only support C# for the moment
            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                "RP0001",
                "Language not supported",
                "ReswPlus source generator only supports C#.",
                "ReswPlus.Errors",
                DiagnosticSeverity.Error,
                true), null));
            return;
        }

        // retrieve the project path
        if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.projectdir", out var projectRootPath))
        {
            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.MSBuildProjectFullPath", out var projectFileFullPath))
            {
                projectRootPath = Path.GetDirectoryName(projectFileFullPath);
            }
            else
            {
                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                   "RP0003",
                   "Root path missing",
                   "Can't retrieve the root path of the project",
                   "ReswPlus.Errors",
                   DiagnosticSeverity.Error,
                   true), null));
                return;
            }
        }

        bool isLibrary = false;
        if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.OutputType", out var outputType))
        {
            isLibrary = outputType.Equals("library", StringComparison.InvariantCultureIgnoreCase)
                || outputType.Equals("module", StringComparison.InvariantCultureIgnoreCase);
        }
        else
        {
            if (context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.projecttypeguids", out var projectTypeGuidsValue))
            {
                isLibrary = projectTypeGuidsValue.Equals("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}", StringComparison.OrdinalIgnoreCase)
                    || projectTypeGuidsValue.Equals("{BC8A1FFA-BEE3-4634-8014-F334798102B3}", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                // Unable to determine if the project is a library; assuming it is an application
                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                    "RP0004",
                    "Unknown Project Type",
                    "ReswPlus cannot determine the project type, defaulting to application.",
                    "ReswPlus.Info",
                    DiagnosticSeverity.Info,
                    isEnabledByDefault: true), Location.None));
            }
        }

        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

        var appType = RetrieveAppType(context);
        switch (appType)
        {
            case AppType.MicrosoftResourceLoader:
                AddSourceFromResource(context, $"{assemblyName}.Templates.ResourceStringProviders.MicrosoftResourceStringProvider.txt", "ResourceStringProvider.cs");
                break;
            case AppType.WindowsResourceLoader:
                AddSourceFromResource(context, $"{assemblyName}.Templates.ResourceStringProviders.WindowsResourceStringProvider.txt", "ResourceStringProvider.cs");
                break;
            default:
                {
                    context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                         "RP0005",
                         "Project type not recognized",
                         "ReswPlus only supports UWP and WinAppSDK applications/libraries.",
                         "ReswPlus.Errors",
                         DiagnosticSeverity.Error,
                         true), null));
                }
                break;
        }

        // retrieve the default language (optional)
        context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.DefaultLanguage", out var projectDefaultLanguage);

        if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var projectRootNamespace))
        {
            // error
            context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
             "RP0002",
             "Unknown namespace",
             "ReswPlus cannot determine the namespace.",
             "ReswPlus.Errors",
             DiagnosticSeverity.Error,
             true), null));
            return;
        }

        var allResourceFiles = (from f in context.AdditionalFiles
                                where Path.GetExtension(f.Path).Equals(".resw", StringComparison.InvariantCultureIgnoreCase)
                                select f).Distinct();
        var defaultLanguageResourceFiles = (from file in allResourceFiles
                                            group file
                                            by Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(file.Path)), Path.GetFileName(file.Path))
                                            into fileGrouped
                                            select RetrieveDefaultResourceFile(fileGrouped.Select(f => f.Path), projectDefaultLanguage)).ToArray();

        var allLanguages = (from path in allResourceFiles select Path.GetFileName(Path.GetDirectoryName(path.Path)).Split('-')[0].ToLower()).Distinct().ToArray();

        foreach (var file in defaultLanguageResourceFiles)
        {
            var namespaceForReswFile = projectRootNamespace;
            var reswParentDirectory = Path.GetDirectoryName(file);
            if (reswParentDirectory.StartsWith(projectRootPath))
            {
                var additionalNamespace = reswParentDirectory.Substring(projectRootPath.Length).Replace(Path.DirectorySeparatorChar, '.');
                if (!string.IsNullOrEmpty(additionalNamespace))
                {
                    namespaceForReswFile += additionalNamespace.StartsWith(".") ? additionalNamespace : "." + additionalNamespace;
                }
            }

            var additionalText = context.AdditionalFiles.FirstOrDefault(f => f.Path == file);
            if (additionalText is null)
            {
                continue;
            }

            var resourceFileInfo = new ResourceFileInfo(file, new Project(context.Compilation.AssemblyName, isLibrary));
            var codeGenerator = ReswClassGenerator.CreateGenerator(resourceFileInfo, null);
            var generatedData = codeGenerator.GenerateCode(
                baseFilename: Path.GetFileName(file).Split('.')[0],
                content: additionalText.GetText(context.CancellationToken).ToString(),
                defaultNamespace: namespaceForReswFile,
                isAdvanced: true);

            foreach (var generatedFile in generatedData.Files)
            {
                var content = generatedFile.Content;
                context.AddSource($"{Path.GetFileName(file)}.cs", SourceText.From(content, Encoding.UTF8));
            }

            if (generatedData.ContainsMacro)
            {
                AddSourceFromResource(context, "ReswPlus.SourceGenerator.Templates.Macros.Macros.txt", "Macros.cs");
            }

            if (generatedData.ContainsPlural)
            {
                AddSourceFromResource(context, $"{assemblyName}.Templates.Plurals.IPluralProvider.txt", "IPluralProvider.cs");
                AddSourceFromResource(context, $"{assemblyName}.Templates.Plurals.PluralTypeEnum.txt", "PluralTypeEnum.cs");
                AddSourceFromResource(context, $"{assemblyName}.Templates.Utils.IntExt.txt", "IntExt.cs");
                AddSourceFromResource(context, $"{assemblyName}.Templates.Utils.DoubleExt.txt", "DoubleExt.cs");
                AddLanguageSupport(context, allLanguages);
            }
        }
    }

    private AppType RetrieveAppType(GeneratorExecutionContext context)
    {
        if (context.Compilation.ExternalReferences.Any(r => r.Display?.IndexOf("Microsoft.WindowsAppSdk", StringComparison.OrdinalIgnoreCase) >= 0))
        {
            return AppType.MicrosoftResourceLoader;
        }

        if (context.Compilation.ExternalReferences.Any(r => r.Display?.IndexOf("Windows.Foundation.UniversalApiContract", StringComparison.OrdinalIgnoreCase) >= 0))
        {
            return AppType.WindowsResourceLoader;
        }

        return AppType.Unknown;
    }

    private static void AddLanguageSupport(GeneratorExecutionContext context, string[] languagesSupported)
    {
        var pluralSelectorCode = "default:\n  return new _ReswPlus_AutoGenerated.Plurals.OtherProvider();\n";
        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        foreach (var pluralFile in PluralFormsProvider.RetrievePluralFormsForLanguages(languagesSupported))
        {
            var pluralFileSource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{assemblyName}.Templates.Plurals.{pluralFile.Id}Provider.txt");
            context.AddSource($"{pluralFile.Id}Provider.cs", SourceText.From(ReadAllText(pluralFileSource), Encoding.UTF8));

            foreach (var lng in pluralFile.Languages)
            {
                pluralSelectorCode += $"case \"{lng}\":\n";
            }
            pluralSelectorCode += $"  return new _ReswPlus_AutoGenerated.Plurals.{pluralFile.Id}Provider();\n";
        }

        var pluralOtherFileSource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{assemblyName}.Templates.Plurals.OtherProvider.txt");
        context.AddSource($"OtherProvider.cs", SourceText.From(ReadAllText(pluralOtherFileSource), Encoding.UTF8));

        var resourceLoaderExtensionSource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"{assemblyName}.Templates.Plurals.ResourceLoaderExtension.txt");
        var resourceLoaderExtensionTemplate = ReadAllText(resourceLoaderExtensionSource);
        var resourceLoaderExtensionCode = resourceLoaderExtensionTemplate.Replace("{{PluralProviderSelector}}", pluralSelectorCode);
        context.AddSource("ResourceLoaderExtension.cs", SourceText.From(resourceLoaderExtensionCode, Encoding.UTF8));
    }

    private static void AddSourceFromResource(GeneratorExecutionContext context, string resourcePath, string itemName)
    {
        var source = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
        context.AddSource(itemName, SourceText.From(ReadAllText(source), Encoding.UTF8));
    }

    private static string ReadAllText(Stream stream)
    {
        _ = stream.Seek(0, SeekOrigin.Begin);
        return new StreamReader(stream).ReadToEnd();
    }
}
