using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml.Linq;

namespace NetSchema.Syntax
{
    internal static class YinSyntaxNodeFactory
    {
        public static bool TryGetSyntaxNode(
            FileInfo file, 
            [NotNullWhen(true)] out ISyntaxNode? syntaxNode
        )
        {
            syntaxNode = default;
            if (file.Extension != ".yin")
                return false;
            var element = XElement.Load(file.FullName);
            syntaxNode = new YinSyntaxNode(null, element);
            return true;
        }
    }
}