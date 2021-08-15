using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NetSchema.Common;

namespace NetSchema.Syntax
{
    internal class MemorySyntaxNode : ISyntaxNode
    {
        public MemorySyntaxNode(StatementType type, string argument, IEnumerable<MemorySyntaxNode> children)
        {
            Type = type;
            Argument = argument;
            this.Children = children.ToList().AsReadOnly();
            foreach (var child in Children)
            {
                child.Parent = this;
            }
        }

        public IReadOnlyList<MemorySyntaxNode> Children { get; }

        public ISyntaxNode? Parent { get; private set; }
        public StatementType Type { get; }
        public string Argument { get; }
        public IEnumerable<ISyntaxNode> GetChildren() => Children;
    }
}