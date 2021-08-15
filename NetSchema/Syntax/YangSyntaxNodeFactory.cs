using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

namespace NetSchema.Syntax
{
    internal static class YangSyntaxNodeFactory
    {
        public static bool TryGetSyntaxNode(FileInfo file, [NotNullWhen(true)] out ISyntaxNode? syntaxNode)
        {
            syntaxNode = default;
            if (file.Extension != ".yang")
                return false;
            var text = File.ReadAllText(file.FullName);
            var tokens = new TokenEnumerator(text);
            syntaxNode = YangParser.Parse(ref tokens);
            return true;
        }
    }
}