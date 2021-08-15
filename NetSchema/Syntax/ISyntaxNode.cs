using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NetSchema.Common;

namespace NetSchema.Syntax
{
    public interface ISyntaxNode
    {
        public ISyntaxNode? Parent { get; }
        public StatementType Type { get; }
        public string Argument { get; }
        public IEnumerable<ISyntaxNode> GetChildren();
    }

    public static class SyntaxNodeExtensions
    {
        public static IEnumerable<ISyntaxNode> GetChildren(this ISyntaxNode node, StatementType type)
            => node.GetChildren().Where(c => c.Type == type);
        public static ISyntaxNode? GetChild(this ISyntaxNode node, StatementType type)
            => node.GetChildren().FirstOrDefault(c => c.Type == type);
        public static string? GetChildArgument(this ISyntaxNode node, StatementType type)
            => node.GetChild(type)?.Argument;

        public static bool TryGetChildArgument(
            this ISyntaxNode node, 
            StatementType type, 
            [NotNullWhen(true)] out string? argument
        )
        {
            argument = node.GetChildArgument(type);
            return argument is not null;
        }
        
        public static bool TryGetChild(
            this ISyntaxNode node, 
            StatementType type, 
            [NotNullWhen(true)] out ISyntaxNode? child
        )
        {
            child = node.GetChild(type);
            return child is not null;
        }
    }
}