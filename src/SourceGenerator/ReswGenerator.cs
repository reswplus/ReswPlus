using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using ReswPlus.Core.ClassGenerator;
using ReswPlus.Core.ResourceInfo;
using ReswPlusSourceGenerator.Models;
using ReswPlusSourceGenerator.Pluralization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ReswPlusSourceGenerator
{

    [Generator]
    public partial class ReswGenerator : ISourceGenerator
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
            if (context.Compilation is not CSharpCompilation csharpCompilation)
            {
                // the converter only support C# for the moment
                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                    "RP0001",
                    "Language not supported",
                    "ReswPlus source generator only supports C#.",
                    "ReswPlus.Errors",
                    DiagnosticSeverity.Error,
                    true), (Location)null));
                return;
            }

            // retrieve the project settings.
            if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.DefaultLanguage", out var projectDefaultLanguage)
                || !context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var projectRootNamespace)
                || !context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.MSBuildProjectFullPath", out var projectFileFullPath)
                || !context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.OutputType", out var outputType))
            {
                // error
                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor(
                 "RP0002",
                 "Build properties missing",
                 "Build properties are not exposed to ReswPlus source generator",
                 "ReswPlus.Errors",
                 DiagnosticSeverity.Error,
                 true), (Location)null));
                return;
            }

#if DEBUG
            if (!Debugger.IsAttached)
            {
                // Uncomment to debug
                // Debugger.Launch();
            }
#endif

            var isLibrary = outputType.Equals("library", StringComparison.InvariantCultureIgnoreCase)
                || outputType.Equals("module", StringComparison.InvariantCultureIgnoreCase);

            var projectRootPath = Path.GetDirectoryName(projectFileFullPath);

            var allResourceFiles = (from f in context.AdditionalFiles
                                    where Path.GetExtension(f.Path).Equals(".resw", StringComparison.InvariantCultureIgnoreCase)
                                    select f.Path).Distinct();
            var defaultLanguageResourceFiles = (from file in allResourceFiles
                                                group file
                                                by Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(file)), Path.GetFileName(file))
                                                into fileGrouped
                                                select RetrieveDefaultResourceFile(fileGrouped, projectDefaultLanguage)).ToArray();

            var allLanguages = (from path in allResourceFiles select Path.GetFileName(Path.GetDirectoryName(path)).Split('-')[0].ToLower()).Distinct().ToArray();

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

                var resourceFileInfo = new ResourceFileInfo(file, new Project(context.Compilation.AssemblyName, isLibrary));
                var codeGenerator = ReswClassGenerator.CreateGenerator(resourceFileInfo, null);
                var generatedData = codeGenerator.GenerateCode(
                    baseFilename: Path.GetFileName(file).Split('.')[0],
                    content: File.ReadAllText(file), defaultNamespace: namespaceForReswFile,
                    isAdvanced: true);

                foreach (var generatedFile in generatedData.Files)
                {
                    var content = generatedFile.Content;
                    context.AddSource($"{Path.GetFileName(file)}.cs", SourceText.From(content, Encoding.UTF8));
                }

                if (generatedData.ContainsMacro)
                {
                    AddSourceFromResource(context, "ReswPlusSourceGenerator.Templates.Macros.Macros.txt", "Macros.cs");
                }
                if (generatedData.ContainsPlural)
                {
                    AddSourceFromResource(context, "ReswPlusSourceGenerator.Templates.Plurals.IPluralProvider.txt", "IPluralProvider.cs");
                    AddSourceFromResource(context, "ReswPlusSourceGenerator.Templates.Plurals.PluralTypeEnum.txt", "PluralTypeEnum.cs");
                    AddSourceFromResource(context, "ReswPlusSourceGenerator.Templates.Utils.IntExt.txt", "IntExt.cs");
                    AddSourceFromResource(context, "ReswPlusSourceGenerator.Templates.Utils.DoubleExt.txt", "DoubleExt.cs");
                    AddLanguageSupport(context, allLanguages);
                }
            }
        }

        private static void AddLanguageSupport(GeneratorExecutionContext context, string[] languagesSupported)
        {
            var pluralFileToAdd = new List<PluralForm>();
            var pluralSelectorCode = "default:\n  return new ReswPlusLib.Providers.OtherProvider();\n";
            foreach (var pluralFile in PluralFormsProvider.RetrievePluralFormsForLanguages(languagesSupported))
            {
                if (!pluralFile.Languages.Any(p => languagesSupported.Contains(p)))
                {
                    continue;
                }
                pluralFileToAdd.Add(pluralFile);
                var pluralFileSource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"ReswPlusSourceGenerator.Templates.Plurals.{pluralFile.Id}Provider.txt");
                context.AddSource($"{pluralFile.Id}Provider.cs", SourceText.From(ReadAllText(pluralFileSource), Encoding.UTF8));

                foreach (var lng in pluralFile.Languages)
                {
                    pluralSelectorCode += $"case \"{lng}\":\n";
                }
                pluralSelectorCode += $"  return new ReswPlusLib.Providers.{pluralFile.Id}Provider();\n";
            }

            var pluralOtherFileSource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"ReswPlusSourceGenerator.Templates.Plurals.OtherProvider.txt");
            context.AddSource($"OtherProvider.cs", SourceText.From(ReadAllText(pluralOtherFileSource), Encoding.UTF8));

            var resourceLoaderExtensionSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("ReswPlusSourceGenerator.Templates.Plurals.ResourceLoaderExtension.txt");
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
            stream.Seek(0, SeekOrigin.Begin);
            return new StreamReader(stream).ReadToEnd();
        }
    }
}
