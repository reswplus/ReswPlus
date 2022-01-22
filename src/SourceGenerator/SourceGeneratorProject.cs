// ------------------------------------------------------
// Copyright (C) Microsoft. All rights reserved.
// ------------------------------------------------------

using ReswPlus.Core.ResourceInfo;

namespace ReswPlusSourceGenerator
{
    public class SourceGeneratorProject : IProject
    {
        public SourceGeneratorProject(string name, bool isLibrary)
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
}
