using System;
using System.Collections.Generic;
using System.IO;
using NetSchema.Syntax;

namespace NetSchema
{
    public record SchemaSettings()
    {
        public static readonly SchemaSettings Default = new SchemaSettings()
        {
            Recursive = true,
        };
        public DirectoryInfo SearchDirectory { get; init; } 
            = new (Environment.CurrentDirectory);
        public bool Recursive { get; init; }
        public IReadOnlyList<string> ModuleNames { get; init; } 
            = Array.Empty<string>();
    };
}