using ReswPlus.Core.ResourceInfo;

namespace ReswPlusSourceGenerator.Models;

public class Project : IProject
{
    public Project(string name, bool isLibrary)
    {
        Name = name;
        IsLibrary = isLibrary;
    }

    public bool IsLibrary { get; }

    public string Name { get; }

    public Language Language => Language.CSHARP;

    public string GetIndentString()
    {
        return "  ";
    }

    public string GetPrecompiledHeader()
    {
        return "";
    }
}
