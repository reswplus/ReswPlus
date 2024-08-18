using System.Collections.Generic;

namespace ReswPlus.Core.ClassGenerator.Models;

public class StronglyTypedClass
{
    public bool IsAdvanced { get; set; }
    public string[] Namespaces { get; set; }
    public string ResoureFile { get; set; }
    public string ClassName { get; set; }

    public List<Localization> Items { get; set; } = new List<Localization>();
}
