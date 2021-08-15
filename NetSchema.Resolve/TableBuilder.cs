using System;
using System.Collections.Generic;
using System.Linq;
using NetSchema.Common;
using NetSchema.Resolve.Tables;
using NetSchema.Syntax;

namespace NetSchema.Resolve
{
    internal static class TableBuilder
    {
        public static GlobalSymbolTable BuildTables(ISyntaxSchema schema)
        {
            var globals = new GlobalSymbolTable();
            foreach (var module in schema.Modules)
            {
                Add(globals, module);
            }
            return globals;
        }

        private static void Add(
            SymbolTable table, 
            ISyntaxNode node
        )
        {
            var family = GetSymbolFamily(node.Type);
            if (family == SymbolFamily.Unknown)
                return;
            (_, table) = table.AddSymbol(node, family);
            foreach (var child in node.GetChildren())
                Add(table, child);
        }

        /*
Identifiers and their namespaces
    Extension: 
        extension
    Feature: 
        feature
    Identity: 
        identity
    Types: 
        typedef
    Grouping: 
        grouping
    Case: 
        All cases within a choice share the same case identifier namespace.  This namespace is scoped to the parent choice node.
    Data: 
        leaf, leaf-list, list, container, choice, anydata, anyxml,       rpc, action, notification
         */
        private static SymbolFamily GetSymbolFamily(StatementType nodeType) => nodeType switch
        {
            StatementType.Module => SymbolFamily.Module,
            StatementType.Submodule => SymbolFamily.Module,
            StatementType.Extension => SymbolFamily.Extension,
            StatementType.Feature => SymbolFamily.Feature,
            StatementType.Identity => SymbolFamily.Identity,
            StatementType.Typedef => SymbolFamily.Type,
            StatementType.Grouping => SymbolFamily.Grouping,
            StatementType.Case => SymbolFamily.Case,
            
            StatementType.Leaf => SymbolFamily.Data,
            StatementType.LeafList => SymbolFamily.Data,
            StatementType.List => SymbolFamily.Data,
            StatementType.Container => SymbolFamily.Data,
            StatementType.Anydata => SymbolFamily.Data,
            StatementType.Anyxml => SymbolFamily.Data,
            StatementType.Choice => SymbolFamily.Data,
            
            StatementType.Unknown => SymbolFamily.Unknown,
            StatementType.ExtensionUsage => SymbolFamily.Unknown,
            StatementType.Action => SymbolFamily.Unknown,
            StatementType.Argument => SymbolFamily.Unknown,
            StatementType.Augment => SymbolFamily.Unknown,
            StatementType.Base => SymbolFamily.Unknown,
            StatementType.BelongsTo => SymbolFamily.Unknown,
            StatementType.Bit => SymbolFamily.Unknown,
            StatementType.Config => SymbolFamily.Unknown,
            StatementType.Contact => SymbolFamily.Unknown,
            StatementType.Default => SymbolFamily.Unknown,
            StatementType.Description => SymbolFamily.Unknown,
            StatementType.Deviate => SymbolFamily.Unknown,
            StatementType.Deviation => SymbolFamily.Unknown,
            StatementType.Enum => SymbolFamily.Unknown,
            StatementType.ErrorAppTag => SymbolFamily.Unknown,
            StatementType.ErrorMessage => SymbolFamily.Unknown,
            StatementType.FractionDigits => SymbolFamily.Unknown,
            StatementType.IfFeature => SymbolFamily.Unknown,
            StatementType.Import => SymbolFamily.Unknown,
            StatementType.Include => SymbolFamily.Unknown,
            StatementType.Input => SymbolFamily.Unknown,
            StatementType.Key => SymbolFamily.Unknown,
            StatementType.Length => SymbolFamily.Unknown,
            StatementType.Mandatory => SymbolFamily.Unknown,
            StatementType.MaxElements => SymbolFamily.Unknown,
            StatementType.MinElements => SymbolFamily.Unknown,
            StatementType.Modifier => SymbolFamily.Unknown,
            StatementType.Must => SymbolFamily.Unknown,
            StatementType.Namespace => SymbolFamily.Unknown,
            StatementType.Notification => SymbolFamily.Unknown,
            StatementType.OrderedBy => SymbolFamily.Unknown,
            StatementType.Organization => SymbolFamily.Unknown,
            StatementType.Output => SymbolFamily.Unknown,
            StatementType.Path => SymbolFamily.Unknown,
            StatementType.Pattern => SymbolFamily.Unknown,
            StatementType.Position => SymbolFamily.Unknown,
            StatementType.Prefix => SymbolFamily.Unknown,
            StatementType.Presence => SymbolFamily.Unknown,
            StatementType.Range => SymbolFamily.Unknown,
            StatementType.Reference => SymbolFamily.Unknown,
            StatementType.Refine => SymbolFamily.Unknown,
            StatementType.RequireInstance => SymbolFamily.Unknown,
            StatementType.Revision => SymbolFamily.Unknown,
            StatementType.RevisionDate => SymbolFamily.Unknown,
            StatementType.Rpc => SymbolFamily.Unknown,
            StatementType.Status => SymbolFamily.Unknown,
            StatementType.Type => SymbolFamily.Unknown,
            StatementType.Unique => SymbolFamily.Unknown,
            StatementType.Units => SymbolFamily.Unknown,
            StatementType.Uses => SymbolFamily.Unknown,
            StatementType.Value => SymbolFamily.Unknown,
            StatementType.When => SymbolFamily.Unknown,
            StatementType.YangVersion => SymbolFamily.Unknown,
            StatementType.YinElement => SymbolFamily.Unknown,
            _ => throw new ArgumentOutOfRangeException(nameof(nodeType), nodeType, null)
        };
    }
}