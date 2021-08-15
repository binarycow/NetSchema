using System.Diagnostics.CodeAnalysis;
using NetSchema.Resolve.Nodes;
using NetSchema.Resolve.Tables;
using NetSchema.Syntax;
using NetSchema.Types;

namespace NetSchema.Resolve
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    internal abstract record Symbol(string Name, SymbolFamily Family, SymbolTable Table)
    {
        public IResolvedNamedSchemaNode? Resolved { get; set; }
        public bool IsResolved => Resolved is not null;
    }

    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    internal record UserSymbol(ISyntaxNode Node, SymbolFamily Family, SymbolTable Table)
        : Symbol(Node.Argument, Family, Table);

    internal record BuiltinTypeSymbol : Symbol
    {
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameterInConstructor")]
        public BuiltinTypeSymbol(GlobalSymbolTable table, IBuiltinSchemaType type) 
            : base(type.QualifiedName.LocalName, SymbolFamily.Type, new OtherSymbolTable(table))
        {
            
        }
    }
}