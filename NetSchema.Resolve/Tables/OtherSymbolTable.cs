using NetSchema.Syntax;

namespace NetSchema.Resolve.Tables
{
    internal class OtherSymbolTable : SymbolTable
    {
        public OtherSymbolTable(SymbolTable parent) => this.Parent = parent;
        public override ISyntaxNode? OriginatingNode => null;
        public override SymbolTable Parent { get; }
        public override GlobalSymbolTable Globals => Parent.Globals;
    }
}