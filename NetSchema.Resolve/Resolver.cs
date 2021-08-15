using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Common.Diagnostics;
using NetSchema.Resolve.Nodes;
using NetSchema.Resolve.Tables;
using NetSchema.Restrictions;
using NetSchema.Syntax;
using NetSchema.Types;

namespace NetSchema.Resolve
{
    internal static class Resolver
    {
        public static IResolvedSchema Resolve(GlobalSymbolTable globalSymbolTable)
        {
            Bind(globalSymbolTable);
            return new ResolvedSchema(globalSymbolTable.Select(s => s.Resolved).OfType<IResolvedSchemaModule>());
        }
        
        private static bool FindAndBindType(SymbolTable table, PrefixedName name, [NotNullWhen(true)] out IUsableType? resolved)
        {
            if (name.Prefix is null && GlobalSymbolTable.TryGetBuiltinType(name.Name, out resolved))
                return true;
            var qName = GetQualifiedName(name, table);
            if(!table.Find<UserSymbol>(SymbolFamily.Type, qName, out var typeTable, out var typeSymbol))
                throw new NotImplementedException();
            if(!BindSymbol<IResolvedSchemaTypedef>(typeTable, typeSymbol, out var derivedType))
                throw new NotImplementedException();
            resolved = derivedType.Type;
            return true;
        }
        
        private static void Bind(SymbolTable table)
        {
            foreach (var symbol in table)
            {
                Bind(symbol.Table);
                if(symbol is UserSymbol userSymbol)
                    BindSymbol(table, userSymbol);
            }
        }

        private static QualifiedName GetQualifiedName(PrefixedName name, SymbolTable symbolTable) => name.Prefix is null
                ? new(symbolTable.ModuleName ?? string.Empty, name.Name)
                : throw new NotImplementedException();

        private static bool BindSymbol(SymbolTable table, UserSymbol symbol)
            => BindSymbol(table, symbol, out _);

        private static bool BindSymbol<TNode>(
            SymbolTable table,
            UserSymbol symbol, 
            [NotNullWhen(true)] out TNode? resolved
        ) where TNode : IResolvedNamedSchemaNode
        {
            resolved = default;
            if (!BindSymbol(table, symbol, out var untyped) || untyped is not TNode typed)
                return false;
            resolved = typed;
            return true;
        }
        private static bool BindSymbol(SymbolTable table, UserSymbol symbol, [NotNullWhen(true)] out IResolvedNamedSchemaNode? resolved)
        {
            resolved = symbol.Node.Type switch
            {
                { } when symbol.Resolved is not null => symbol.Resolved,
                StatementType.Leaf => BindLeaf(table, symbol.Node, symbol),
                StatementType.Container => BindContainer(table, symbol.Node, symbol),
                StatementType.LeafList => BindLeafList(table, symbol.Node, symbol),
                StatementType.List => BindList(table, symbol.Node, symbol),
                StatementType.Type => BindType(table, symbol.Node, symbol),
                StatementType.Module => BindModule(table, symbol.Node, symbol),
                StatementType.Typedef => BindTypedef(table, symbol.Node, symbol),
                _ => throw new NotImplementedException(),
            };
            symbol.Resolved = resolved;
            return resolved is not null;
        }


        private static IResolvedSchemaModule? BindModule(
            SymbolTable table, 
            ISyntaxNode node, 
            UserSymbol symbol
        )
        {
            var dataNodes = GetDataNodes(symbol.Table);
            var ns = (XNamespace)(node.GetChildArgument(StatementType.Namespace) ?? throw new NotImplementedException());
            var prefix = node.GetChildArgument(StatementType.Prefix) ?? throw new NotImplementedException();
            return new ResolvedSchemaModule(
                ns + node.Argument,
                prefix,
                dataNodes
            )
            {
                Version = OptionalValue.CreateVersion(node.GetChildArgument(StatementType.YangVersion)),
            };
        }

        private static IResolvedSchemaType? BindType(
            SymbolTable parentTable,
            ISyntaxNode node,
            UserSymbol symbol
        )
        {
            if (!PrefixedName.TryParse(node.Argument, out var pName))
                throw new NotImplementedException();
            if(!FindAndBindType(parentTable, pName, out var type))
                throw new NotImplementedException();
            var restrictions = GetRestrictions(type.Kind, node).ToList().AsReadOnly();
            return new ResolvedSchemaType(type)
            {
                Restrictions = restrictions,
            };


            static IEnumerable<ITypeRestriction> GetRestrictions(TypeKind kind, ISyntaxNode node) 
                => node.GetChildren().Where(GetRestrictionPredicate(kind)).Select(ParseRestriction);

            static Func<ISyntaxNode, bool> GetRestrictionPredicate(TypeKind kind) => kind switch
            {
                TypeKind.String => IsValidStringRestriction,
                TypeKind.Boolean => IsValidBoolRestriction,
                _ => throw new NotImplementedException()
            };

            static bool IsValidStringRestriction(ISyntaxNode node) => node.Type is StatementType.Pattern or StatementType.Length;
            static bool IsValidBoolRestriction(ISyntaxNode node) => false;
        }

        private static ITypeRestriction ParseRestriction(ISyntaxNode syntaxNode)
        {
            return syntaxNode.Type switch
            {
                StatementType.Pattern => ParsePattern(syntaxNode),
                _ => throw new NotImplementedException(),
            };

            static IReadOnlySchemaPattern ParsePattern(ISyntaxNode syntaxNode) 
                => new ReadOnlySchemaPattern(new (syntaxNode.Argument))
            {
                ErrorMessage = syntaxNode.GetChildArgument(StatementType.ErrorMessage),
                ErrorAppTag = syntaxNode.GetChildArgument(StatementType.ErrorAppTag),
                Description = syntaxNode.GetChildArgument(StatementType.Description),
                Reference = syntaxNode.GetChildArgument(StatementType.Reference),
                Modifier = OptionalValue.CreatePatternModifier(syntaxNode.GetChildArgument(StatementType.Modifier)),
            };
        }

        private static IResolvedNamedSchemaNode BindTypedef(SymbolTable table, ISyntaxNode node, UserSymbol symbol)
        {
            var typeStatement = node.GetChild(StatementType.Type)
                ?? throw new NotImplementedException();
            var baseType = BindType(table, typeStatement, symbol)
                ?? throw new NotImplementedException();
            return new ResolvedSchemaTypedef(node.Argument, baseType.GetUsableType(MakeQualifiedName(node.Argument, table)));
        }


        private static IResolvedSchemaLeaf? BindLeaf(
            SymbolTable parentTable,
            ISyntaxNode node, 
            UserSymbol symbol
        )
        {
            if(!PrefixedName.TryParse(node.GetChildArgument(StatementType.Type), out var pName))
                throw new NotImplementedException();
            if(!FindAndBindType(symbol.Table, pName, out var type))
                throw new NotImplementedException();
            return new ResolvedSchemaLeaf(node.Argument, type)
            {
                Config = OptionalValue.CreateConfig(node.GetChildArgument(StatementType.Config)),
                Description = node.GetChildArgument(StatementType.Description),
                Reference = node.GetChildArgument(StatementType.Reference),
            };
        }


        private static IResolvedSchemaContainer? BindContainer(
            SymbolTable parentTable, 
            ISyntaxNode node,
            UserSymbol symbol
        )
        {
            var dataNodes = GetDataNodes(symbol.Table);
            return new ResolvedSchemaContainer(node.Argument, dataNodes);
        }
        private static IResolvedSchemaList? BindList(
            SymbolTable parentTable, 
            ISyntaxNode node, 
            UserSymbol symbol
        )
        {
            var dataNodes = GetDataNodes(symbol.Table).ToList();
            var keyArg = node.GetChildArgument(StatementType.Key);
            var keyArgs = keyArg is not null ? PrefixedName.ParseList(keyArg) : null;
            var keys = GetKeys(dataNodes, keyArgs, parentTable);
            return new ResolvedSchemaList(node.Argument, dataNodes, keys)
            {
                Config = OptionalValue.CreateConfig(node.GetChildArgument(StatementType.Config)),
                Description = node.GetChildArgument(StatementType.Description),
                Reference = node.GetChildArgument(StatementType.Reference),
            };

            static IEnumerable<IResolvedSchemaLeaf> GetKeys(
                IEnumerable<IResolvedSchemaDataNode> dataNodes,
                IReadOnlyCollection<PrefixedName>? keys, 
                SymbolTable table
            )
            {
                if (keys is null || keys.Count == 0)
                {
                    return Enumerable.Empty<IResolvedSchemaLeaf>();
                }
                var keyNames = keys.Select(x => GetQualifiedName(x, table)).ToHashSet();
                return dataNodes.OfType<IResolvedSchemaLeaf>()
                    .Where(n => keyNames.Contains(MakeQualifiedName(n.QualifiedName, table)));
            }
        }

        private static QualifiedName MakeQualifiedName(QualifiedName name, SymbolTable table) 
            => name.ModuleName.IsNullOrWhiteSpace()
                ? new(table.GetModule()?.Argument ?? string.Empty, name.LocalName) 
                : name;
        private static QualifiedName MakeQualifiedName(string name, SymbolTable table) 
            => new(table.GetModule()?.Argument ?? string.Empty, name) ;

        private static IResolvedSchemaLeafList? BindLeafList(
            SymbolTable parentTable,
            ISyntaxNode node, 
            UserSymbol symbol
        )
        {
            if (!PrefixedName.TryParse(node.GetChildArgument(StatementType.Type), out var pName))
                throw new NotImplementedException();
            if(!FindAndBindType(symbol.Table, pName, out var type))
                throw new NotImplementedException();
            return new ResolvedSchemaLeafList(node.Argument, type)
            {
                Config = OptionalValue.CreateConfig(node.GetChildArgument(StatementType.Config)),
                Description = node.GetChildArgument(StatementType.Description),
                Reference = node.GetChildArgument(StatementType.Reference),
            };
        }

        private static IEnumerable<IResolvedSchemaDataNode> GetDataNodes(IEnumerable<Symbol> symbols)
        {
            foreach (var symbol in symbols)
            {
                var resolved = symbol.Resolved switch
                {
                    null => throw new NotImplementedException(),
                    IResolvedSchemaDataNode dataNode => dataNode,
                    IResolvedSchemaTypedef => null,
                    _ => throw new NotImplementedException()
                };
                if (resolved is not null)
                    yield return resolved;
            }
        }
    }
}