using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ReswPlus.Core.Interfaces;
using ReswPlus.Core.ResourceParser;
using ReswPlus.SourceGenerator.ClassGenerators.Models;
using ReswPlus.SourceGenerator.CodeGenerators;
using ReswPlus.SourceGenerator.Models;

namespace ReswPlus.SourceGenerator.ClassGenerators;

public class ReswClassGenerator
{
    private const string TagIgnore = "#ReswPlusIgnore";
    private const string Deprecated_TagStrongType = "#ReswPlusTyped";
    private const string TagFormat = "#Format";
    private const string TagFormatDotNet = "#FormatNet";

    private static readonly Regex _regexStringFormat;
    private static readonly Regex _regexRemoveSpace = new("\\s+");
    private readonly ResourceFileInfo _resourceFileInfo;
    private readonly ICodeGenerator _codeGenerator;
    private readonly IErrorLogger _logger;

    static ReswClassGenerator()
    {
        _regexStringFormat =
            new Regex(
                $"(?<tag>{TagFormat}|{TagFormatDotNet})\\[" +
                "(?<formats>(?:" +
                     "[^\\\"]|" +
                     "\"(?:[^\\\"]|\\\\.)*\"" +
                     ")+" +
                   ")" +
                "\\]");
    }

    private ReswClassGenerator(ResourceFileInfo resourceInfo, ICodeGenerator generator, IErrorLogger logger)
    {
        _resourceFileInfo = resourceInfo;
        _codeGenerator = generator;
        _logger = logger;
    }

    internal static ReswClassGenerator CreateGenerator(ResourceFileInfo resourceFileInfo, IErrorLogger logger)
    {
        ICodeGenerator codeGenerator = null;
        switch (resourceFileInfo.Project.Language)
        {
            case Language.CSharp:
                codeGenerator = new CSharpCodeGenerator();
                break;
        }

        return codeGenerator is not null ? new ReswClassGenerator(resourceFileInfo, codeGenerator, logger) : null;
    }

    private StronglyTypedClass Parse(string content, string defaultNamespace, bool isAdvanced, AppType appType)
    {
        var namespaceToUse = ExtractNamespace(defaultNamespace);
        var resourceFileName = Path.GetFileName(_resourceFileInfo.Path);
        var className = Path.GetFileNameWithoutExtension(_resourceFileInfo.Path);
        var reswInfo = ReswParser.Parse(content);

        var projectNameIfLibrary = _resourceFileInfo.Project.IsLibrary ? _resourceFileInfo.Project.Name : null;

        //If the resource file is in a library, the resource id in the .pri file
        //will be <library name>/FilenameWithoutExtension
        var resouceNameForResourceLoader = string.IsNullOrEmpty(projectNameIfLibrary) ?
            className : projectNameIfLibrary + "/" + className;

        var result = new StronglyTypedClass()
        {
            IsAdvanced = isAdvanced,
            ClassName = className,
            Namespaces = namespaceToUse,
            ResoureFile = resouceNameForResourceLoader,
            AppType = appType
        };

        var stringItems = reswInfo.Items
            .Where(i => IsValidPropertyName(i.Key) && !(i.Comment?.Contains(TagIgnore) ?? false))
            .ToArray();

        if (isAdvanced)
        {
            //check Pluralization
            var itemsWithPluralOrVariant = reswInfo.Items.GetItemsWithVariantOrPlural();
            var basicItems = stringItems.Except(itemsWithPluralOrVariant.SelectMany(e => e.Items)).ToArray();

            foreach (var item in itemsWithPluralOrVariant)
            {
                var itemKey = item.Key;
                if (item.SupportPlural)
                {
                    var idNone = $"{itemKey}_None";
                    var hasNoneForm = reswInfo.Items.Any(i => i.Key == idNone);

                    var singleLineValue = _regexRemoveSpace.Replace(item.Items.FirstOrDefault().Value, " ").Trim();

                    var summary = $"Get the pluralized version of the string similar to: {singleLineValue}";

                    var localization = item.SupportVariants
                        ? new PluralVariantLocalization()
                        {
                            Key = itemKey,
                            Summary = summary,
                            SupportNoneState = hasNoneForm,
                        }
                        : new PluralLocalization()
                        {
                            Key = itemKey,
                            Summary = summary,
                            SupportNoneState = hasNoneForm,
                        };
                    if (item.Items.Any(i => i.Comment != null && i.Comment.Contains(Deprecated_TagStrongType)))
                    {
                        _logger?.LogError($"{Deprecated_TagStrongType} is no more supported, use {TagFormat} instead. See https://github.com/DotNetPlus/ReswPlus/blob/master/README.md");
                    }
                    var commentToUse =
                        item.Items.FirstOrDefault(i => i.Comment != null && _regexStringFormat.IsMatch(i.Comment));

                    _ = ManageFormattedFunction(localization, commentToUse?.Comment, basicItems, resourceFileName);

                    result.Items.Add(localization);
                }
                else if (item.SupportVariants)
                {
                    var singleLineValue = _regexRemoveSpace.Replace(item.Items.FirstOrDefault().Value, " ").Trim();
                    var summary = $"Get the variant version of the string similar to: {singleLineValue}";
                    var commentToUse = item.Items.FirstOrDefault(i => i.Comment != null && _regexStringFormat.IsMatch(i.Comment));

                    var localization = new VariantLocalization()
                    {
                        Key = itemKey,
                        Summary = summary,
                    };

                    _ = ManageFormattedFunction(localization, commentToUse?.Comment, basicItems, resourceFileName);

                    result.Items.Add(localization);
                }
            }

            stringItems = basicItems;
        }

        if (stringItems.Any())
        {
            foreach (var item in stringItems)
            {
                var itemKey = item.Key;
                var singleLineValue = _regexRemoveSpace.Replace(item.Value, " ").Trim();
                var summary = $"Looks up a localized string similar to: {singleLineValue}";

                var localization = new RegularLocalization()
                {
                    Key = itemKey,
                    Summary = summary,
                };

                if (isAdvanced)
                {
                    _ = ManageFormattedFunction(localization, item.Comment, stringItems, resourceFileName);
                }
                result.Items.Add(localization);
            }
        }

        return result;
    }

    private static bool IsValidPropertyName(string propertyName)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            return false;
        }

        if (!char.IsLetter(propertyName[0]) && propertyName[0] != '_')
        {
            return false;
        }

        for (int i = 1; i < propertyName.Length; i++)
        {
            if (!char.IsLetterOrDigit(propertyName[i]) && propertyName[i] != '_')
            {
                return false;
            }
        }

        return true;
    }

    internal GenerationResult GenerateCode(string baseFilename, string content, string defaultNamespace, bool isAdvanced, AppType appType)
    {
        var stronglyTypedClassInfo = Parse(content, defaultNamespace, isAdvanced, appType);
        if (stronglyTypedClassInfo is null)
        {
            return null;
        }

        var filesGenerated = _codeGenerator.GetGeneratedFiles(baseFilename, stronglyTypedClassInfo, _resourceFileInfo);
        var result = new GenerationResult()
        {
            Files = filesGenerated
        };

        if (filesGenerated is not null && filesGenerated.Any())
        {
            result.ContainsPlural = stronglyTypedClassInfo.Items.Any(l => l is PluralLocalization);
            result.ContainsMacro = stronglyTypedClassInfo.Items.Any(l => l.Parameters.Any(p => p is MacroFormatTagParameter));
        }
        return result;
    }

    private string[] ExtractNamespace(string defaultNamespace)
    {
        if (string.IsNullOrEmpty(defaultNamespace))
        {
            return Array.Empty<string>();
        }

        // remove bcp47 tag from the namespace
        var splittedNamespace = defaultNamespace.Split('.');
        var lastNamespace = splittedNamespace.Last().Replace('_', '-');
        var culture = CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(c => string.Compare(c.Name, lastNamespace, true) == 0);
        return culture is not null ? splittedNamespace.Take(splittedNamespace.Length - 1).ToArray() : splittedNamespace;
    }

    public static (string format, bool isDotNetFormatting) ParseTag(string comment)
    {
        if (!string.IsNullOrWhiteSpace(comment))
        {
            var match = _regexStringFormat.Match(comment);
            if (match.Success)
            {
                var tag = match.Groups["tag"].Value;
                return (match.Groups["formats"].Value.Trim(), tag == TagFormatDotNet);
            }
        }
        return (null, false);
    }

    private bool ManageFormattedFunction(Localization localization, string comment, IEnumerable<ReswItem> basicLocalizedItems, string resourceName)
    {
        FunctionFormatTagParametersInfo tagTypedInfo = null;
        var (format, isDotNetFormatting) = ParseTag(comment);
        if (format != null)
        {
            localization.IsDotNetFormatting = isDotNetFormatting;
            var types = FormatTag.SplitParameters(format);
            tagTypedInfo = FormatTag.ParseParameters(localization.Key, types, basicLocalizedItems, resourceName, _logger);
            if (tagTypedInfo != null)
            {
                localization.Parameters = tagTypedInfo.Parameters;
            }
        }

        if (localization is IVariantLocalization variantLocalization)
        {
            FunctionFormatTagParameter variantParameter;
            // Add an extra parameter for variant if necessary
            if (tagTypedInfo?.VariantParameter == null)
            {
                variantParameter = new FunctionFormatTagParameter
                { Type = ParameterType.Long, Name = "variantId", IsVariantId = true };
                localization.ExtraParameters.Add(variantParameter);
            }
            else
            {
                variantParameter = tagTypedInfo.VariantParameter;
            }

            variantLocalization.ParameterToUseForVariant = variantParameter;

        }

        if (localization is PluralLocalization pluralLocalization)
        {
            FunctionFormatTagParameter pluralizationQuantifier;
            // Add an extra parameter for pluralization if necessary
            if (tagTypedInfo?.PluralizationParameter == null)
            {
                pluralizationQuantifier = new FunctionFormatTagParameter
                { Type = ParameterType.Double, Name = "pluralizationReferenceNumber" };
                pluralLocalization.ExtraParameters.Add(pluralizationQuantifier);
            }
            else
            {
                pluralizationQuantifier = tagTypedInfo.PluralizationParameter;
            }

            pluralLocalization.ParameterToUseForPluralization = pluralizationQuantifier;
        }

        return true;
    }

}
