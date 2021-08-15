#nullable enable

using System.Collections.Generic;
using System.Linq;
using NetSchema.Common;
using NetSchema.Common.Collections;
using NetSchema.Data.Nodes;
using NetSchema.Resolve.Nodes;

namespace NetSchema.Data
{
    
    
    
    internal class KeyedListChildren : NsKeyedCollection<ListKeySet, IDataKeyedListItem>
    {
        public KeyedListChildren(IResolvedSchemaList schemaNode) : base(ListKeySet.EqualityComparer)
        {
            this.KeyNames = this.GetKeyNames(schemaNode);
        }
        
        private IReadOnlyList<QualifiedName> GetKeyNames(IResolvedSchemaList schemaNode)
        {
            return schemaNode.Keys.Select(x => x.QualifiedName).ToList().AsReadOnly();
        }

        public IReadOnlyList<QualifiedName> KeyNames { get; }
        
        protected override ListKeySet GetKeyForItem(IDataKeyedListItem item) 
            => ListKeySet.Create(item, this.KeyNames);
    }
}