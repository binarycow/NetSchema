using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NetSchema.Common;
using NetSchema.Common.Collections;
using NetSchema.Data.Collections;
using NetSchema.Syntax;

namespace NetSchema.Resolve.Tables
{
    internal abstract class SymbolTable : NsKeyedCollection<(SymbolFamily Family, string Name), Symbol>
    {
        public abstract ISyntaxNode? OriginatingNode { get; }
        public abstract SymbolTable? Parent { get; }
        public abstract GlobalSymbolTable Globals { get; }

        protected SymbolTable() : base(EqualityComparer<(SymbolFamily Family, string Name)>.Default)
        {

        }

        public string? ModuleName => this.GetModule()?.Argument;

        public ISyntaxNode? GetModule()
        {
            var current = this;
            while (current is not null)
            {
                if (current.OriginatingNode?.Type == StatementType.Module)
                    return current.OriginatingNode;
                current = current.Parent;
            }
            return null;
        }
        
        protected override (SymbolFamily Family, string Name) GetKeyForItem(Symbol item)
            => (item.Family, item.Name);

        
        public (Symbol Symbol, SymbolTable Table) AddSymbol(ISyntaxNode node, SymbolFamily family)
        {
            var table = new ChildSymbolTable(node, this);
            var symbol = new UserSymbol(node, family, table);
            this.Add(symbol);
            return (symbol, table);
        }

        public bool Find<TSymbol>(
            SymbolFamily family,
            QualifiedName qName,
            [NotNullWhen(true)] out SymbolTable? symbolTable,
            [NotNullWhen(true)] out TSymbol? symbol
        ) where TSymbol : Symbol
        {
            symbol = default;
            if (!Find(family, qName, out symbolTable, out var untypedSymbol) || untypedSymbol is not TSymbol typedSymbol)
                return false;
            symbol = typedSymbol;
            return true;
        }
        
        public bool Find(
            SymbolFamily family,
            QualifiedName qName,
            [NotNullWhen(true)] out SymbolTable? symbolTable,
            [NotNullWhen(true)] out Symbol? symbol
        )
        {
            if(qName.ModuleName != this.ModuleName)
                throw new NotImplementedException();
            return FindThisModule(family, qName.LocalName, out symbolTable, out symbol);
        }

        private bool FindThisModule(
            SymbolFamily family,
            string name,
            [NotNullWhen(true)] out SymbolTable? symbolTable,
            [NotNullWhen(true)] out Symbol? symbol
        )
        {
            symbol = default;
            symbolTable = this;
            var key = (family, name);
            while (symbolTable is not null && symbolTable is not GlobalSymbolTable)
            {
                if (symbolTable.TryGetValue(key, out symbol))
                {
                    return true;
                }
                symbolTable = symbolTable.Parent;
            }
            return false;
        }
    }
}