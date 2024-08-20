namespace ReswPlus.SourceGenerator.Models;

internal class Project : IProject
{
    public Project(string name, bool isLibrary)
    {
        Name = name;
        IsLibrary = isLibrary;
    }

    public bool IsLibrary { get; }

    public string Name { get; }

    public Language Language => Language.CSharp;

    public string GetIndentString()
    {
        return "  ";
    }

    public string GetPrecompiledHeader()
    {
        return "";
    }
}
