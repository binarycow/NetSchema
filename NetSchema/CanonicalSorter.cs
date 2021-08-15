using System;
using System.Collections.Generic;
using NetSchema.Common;
using NetSchema.Syntax;

using static NetSchema.Common.StatementType;

namespace NetSchema
{
    public class CanonicalSorter : IComparer<ISyntaxNode>
    {
        private CanonicalSorter()
        {
            
        }
        private static readonly Lazy<CanonicalSorter> instance = new (() => new ());
        public static CanonicalSorter Instance => instance.Value!; // Null-forgiving

        private static readonly List<StatementType> canonicalOrder = new ()
        {
            StatementType.YangVersion,
            Namespace,
            Prefix,
            Import,
            Include,
            Organization,
            Contact,
            Description,
            Reference,
            Revision,
            Extension,
            Feature,
            Identity,
            Typedef,
            Grouping,
            Container,
            Leaf,
            LeafList,
            List,
            Choice,
            Anydata,
            Anyxml,
            Uses,
            Augment,
            Rpc,
            Notification,
            Deviation,
        };


        public int Compare(ISyntaxNode x, ISyntaxNode y) 
            => GetCanonicalOrder(x.Type).CompareTo(GetCanonicalOrder(y.Type));

        public static int GetCanonicalOrder(StatementType type) => canonicalOrder.IndexOf(type);
    }
}