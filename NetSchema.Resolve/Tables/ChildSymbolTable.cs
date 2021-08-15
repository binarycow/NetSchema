using NetSchema.Syntax;

namespace NetSchema.Resolve.Tables
{
    internal class ChildSymbolTable : SymbolTable
    {
        public ChildSymbolTable(ISyntaxNode? originatingNode, SymbolTable parent)
        {
            this.OriginatingNode = originatingNode;
            this.Parent = parent;
        }

        public override ISyntaxNode? OriginatingNode { get; }
        public override SymbolTable Parent { get; }
        public override GlobalSymbolTable Globals => Parent.Globals;
    }
}