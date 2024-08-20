namespace ReswPlus.SourceGenerator.Models;

internal interface IProject
{
    bool IsLibrary { get; }
    string Name { get; }
    Language Language { get; }
    string GetPrecompiledHeader();
    string GetIndentString();
}
