using System.Collections.Generic;
using ReswPlus.Core.ClassGenerator.Models;
using ReswPlus.Core.ResourceInfo;

namespace ReswPlus.Core.CodeGenerators;

public class GeneratedFile
{
    public string Filename { get; set; }
    public string Content { get; set; }
    public string[] Languages { get; set; }

}

public interface ICodeGenerator
{
    IEnumerable<GeneratedFile> GetGeneratedFiles(string baseFilename, StronglyTypedClass info, ResourceFileInfo projectItem);
}
