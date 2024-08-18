using System.Collections.Generic;
using ReswPlus.Core.CodeGenerators;

namespace ReswPlus.Core.ClassGenerator;

public class GenerationResult
{
    public IEnumerable<GeneratedFile> Files { get; set; }
    public bool ContainsPlural { get; set; }
    public bool ContainsMacro { get; set; }
}
