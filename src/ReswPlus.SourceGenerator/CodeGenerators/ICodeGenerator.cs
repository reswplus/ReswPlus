using System.Collections.Generic;
using ReswPlus.SourceGenerator.ClassGenerators.Models;
using ReswPlus.SourceGenerator.Models;

namespace ReswPlus.SourceGenerator.CodeGenerators;

internal class GeneratedFile
{
    public string Filename { get; set; }
    public string Content { get; set; }
    public string[] Languages { get; set; }

}

internal interface ICodeGenerator
{
    IEnumerable<GeneratedFile> GetGeneratedFiles(string baseFilename, StronglyTypedClass info, ResourceFileInfo projectItem);
}
