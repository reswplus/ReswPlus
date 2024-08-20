using System.Collections.Generic;
using System.Linq;
using ReswPlus.Core.ResourceParser;

namespace ReswPlus.SourceGenerator.ClassGenerators.Models;

internal abstract class Localization
{
    public string Key { get; set; }
    public List<IFormatTagParameter> Parameters { get; set; } = [];
    public List<FunctionFormatTagParameter> ExtraParameters { get; } = [];
    public string Summary { get; set; }
    public bool IsDotNetFormatting { get; set; }
    public bool IsProperty => !Parameters.OfType<FunctionFormatTagParameter>().Any() && !ExtraParameters.Any();
}

internal class RegularLocalization : Localization
{ }

internal class PluralLocalization : Localization
{
    public bool SupportNoneState { get; set; }
    public FunctionFormatTagParameter ParameterToUseForPluralization { get; set; }
}

internal interface IVariantLocalization
{
    FunctionFormatTagParameter ParameterToUseForVariant { get; set; }
}

internal class PluralVariantLocalization : PluralLocalization, IVariantLocalization
{
    public FunctionFormatTagParameter ParameterToUseForVariant { get; set; }
}

internal class VariantLocalization : Localization, IVariantLocalization
{
    public FunctionFormatTagParameter ParameterToUseForVariant { get; set; }
}
