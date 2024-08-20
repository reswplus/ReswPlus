using System.Collections.Generic;
using ReswPlus.SourceGenerator.CodeGenerators;

namespace ReswPlus.SourceGenerator.ClassGenerators;

internal class GenerationResult
{
    public IEnumerable<GeneratedFile> Files { get; set; }
    public bool ContainsPlural { get; set; }
    public bool ContainsMacro { get; set; }
}
