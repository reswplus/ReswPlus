// ------------------------------------------------------
// Copyright (C) Microsoft. All rights reserved.
// ------------------------------------------------------

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

namespace ReswPlusSourceGenerator
{

    [Generator]
    public partial class SidecarConnectorGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        private string SelectDefaultLanguage(IEnumerable<string> reswFiles, string defaultLanguage)
        {

            var languages = new List<string>();
            if (!string.IsNullOrEmpty(defaultLanguage))
            {
                languages.Add(defaultLanguage);
            }
            if ("en-us".Equals(defaultLanguage, StringComparison.InvariantCultureIgnoreCase))
            {
                languages.Add("en-us");
            }
            if ("en".Equals(defaultLanguage, StringComparison.InvariantCultureIgnoreCase))
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

            return reswFiles.FirstOrDefault();
        }

        public void Execute(GeneratorExecutionContext context)
        {

            if (context.Compilation is not CSharpCompilation csharpCompilation)
            {
                // the converter only support C# for the moment
                return;
            }

            if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.DefaultLanguage", out var defaultLanguage))
            {
                //error
                return;
            }

            if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.RootNamespace", out var rootNamespace))
            {
                // error
                return;
            }

            if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.MSBuildProjectFullPath", out var projectFileFullPath))
            {
                // error
                return;
            }

            if (!context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.OutputType", out var outputType))
            {
                // error
                return;
            }

#if DEBUG
            if (!Debugger.IsAttached)
            {
             //   Debugger.Launch();
            }
#endif

            var isLibrary = outputType.Equals("library", StringComparison.InvariantCultureIgnoreCase)
                || outputType.Equals("module", StringComparison.InvariantCultureIgnoreCase);

            var projectRoot = Path.GetDirectoryName(projectFileFullPath);

            var allResourceFiles = (from f in context.AdditionalFiles
                                    where Path.GetExtension(f.Path).Equals(".resw", StringComparison.InvariantCultureIgnoreCase)
                                    select f.Path).Distinct();
            var defaultLanguageResourceFiles = (from file in allResourceFiles
                                                group file
                                                by Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(file)), Path.GetFileName(file))
                                                into fileGrouped
                                                select SelectDefaultLanguage(fileGrouped, defaultLanguage)).ToArray();

            var allLanguages = (from path in allResourceFiles select Path.GetFileName(Path.GetDirectoryName(path)).Split('-')[0].ToLower()).Distinct().ToArray();

            foreach (var file in defaultLanguageResourceFiles)
            {
                var namespaceForReswFile = rootNamespace;
                var reswParentDirectory = Path.GetDirectoryName(file);
                if (reswParentDirectory.StartsWith(projectRoot))
                {
                    var additionalNamespace = reswParentDirectory.Substring(projectRoot.Length).Replace(Path.DirectorySeparatorChar, '.');
                    if (!string.IsNullOrEmpty(additionalNamespace))
                    {
                        namespaceForReswFile += additionalNamespace.StartsWith(".") ? additionalNamespace : "." + additionalNamespace;
                    }
                }

                var resourceFileInfo = new ResourceFileInfo(file, new SourceGeneratorProject(context.Compilation.AssemblyName, isLibrary));
                var codeGenerator = ReswClassGenerator.CreateGenerator(resourceFileInfo, null);
                var generatedData = codeGenerator.GenerateCode(
                    baseFilename:Path.GetFileName(file).Split('.')[0],
                    content:File.ReadAllText(file), defaultNamespace: namespaceForReswFile,
                    isAdvanced: true);

                foreach (var generatedFile in generatedData.Files)
                {
                    var content = generatedFile.Content;
                    context.AddSource($"{Path.GetFileName(file)}.cs", SourceText.From(content, Encoding.UTF8));
                }

                if (generatedData.ContainsMacro)
                {
                    var resourceLoaderExtensionSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("ReswPlusSourceGenerator.Templates.Macros.Macros.txt");
                    context.AddSource("Macros.cs", SourceText.From(ReadAllText(resourceLoaderExtensionSource), Encoding.UTF8));
                }
                if (generatedData.ContainsPlural)
                {
                    var iPluralFileSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("ReswPlusSourceGenerator.Templates.Plurals.IPluralProvider.txt");
                    context.AddSource("IPluralProvider.cs", SourceText.From(ReadAllText(iPluralFileSource), Encoding.UTF8));
                    var pluralTypeSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("ReswPlusSourceGenerator.Templates.Plurals.PluralTypeEnum.txt");
                    context.AddSource("PluralTypeEnum.cs", SourceText.From(ReadAllText(pluralTypeSource), Encoding.UTF8));
                    var intExtSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("ReswPlusSourceGenerator.Templates.Utils.IntExt.txt");
                    context.AddSource("IntExt.cs", SourceText.From(ReadAllText(intExtSource), Encoding.UTF8));
                    var doubleExtSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("ReswPlusSourceGenerator.Templates.Utils.DoubleExt.txt");
                    context.AddSource("DoubleExt.cs", SourceText.From(ReadAllText(doubleExtSource), Encoding.UTF8));

                    AddLanguageSupport(context, allLanguages);
                }
            }
        }

        private void AddLanguageSupport(GeneratorExecutionContext context, string[] languagesSupported)
        {
            var pluralFileToAdd = new List<PluralForm>();
            var pluralSelectorCode = "default:\n  return new ReswPlusLib.Providers.OtherProvider();\n";
            foreach (var pluralFile in Pluralizations.PluralForms)
            {
                if (!pluralFile.Languages.Any(p => languagesSupported.Contains(p)))
                {
                    continue;
                }
                pluralFileToAdd.Add(pluralFile);
                var pluralFileSource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"ReswPlusSourceGenerator.Templates.Plurals.{pluralFile.Name}Provider.txt");
                context.AddSource($"{pluralFile.Name}Provider.cs", SourceText.From(ReadAllText(pluralFileSource), Encoding.UTF8));

                foreach (var lng in pluralFile.Languages)
                {
                    pluralSelectorCode += $"case \"{lng}\":\n";
                }
                pluralSelectorCode += $"  return new ReswPlusLib.Providers.{pluralFile.Name}Provider();\n";
            }


            var pluralOtherFileSource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"ReswPlusSourceGenerator.Templates.Plurals.OtherProvider.txt");
            context.AddSource($"OtherProvider.cs", SourceText.From(ReadAllText(pluralOtherFileSource), Encoding.UTF8));

            var resourceLoaderExtensionSource = Assembly.GetExecutingAssembly().GetManifestResourceStream("ReswPlusSourceGenerator.Templates.Plurals.ResourceLoaderExtension.txt");
            var resourceLoaderExtensionTemplate = ReadAllText(resourceLoaderExtensionSource);
            var resourceLoaderExtensionCode = resourceLoaderExtensionTemplate.Replace("{{PluralProviderSelector}}", pluralSelectorCode);
            context.AddSource("ResourceLoaderExtension.cs", SourceText.From(resourceLoaderExtensionCode, Encoding.UTF8));
        }

        private string ReadAllText(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return new StreamReader(stream).ReadToEnd();
        }
    }
}
