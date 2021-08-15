#nullable enable

using System.Collections.Generic;
using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Common.Collections;
using NetSchema.Resolve.Nodes;

namespace NetSchema.Data.Nodes
{
    public interface IDataKeyedList : IDataList
    {
        public IReadOnlyList<QualifiedName> KeyNames { get; }
        IReadOnlyKeyedCollection<ListKeySet, IDataKeyedListItem> Children { get; }
        Result TryAddChild(IDataKeyedListItem child);
    }

    
    public static class ResolvedSchemaListExtensions
    {
        public static ListKeySet CreateKey(this IDataKeyedList list, IEnumerable<(QualifiedName Name, object? Value)> values)
        {
            return ListKeySet.Create(values, list);
        }
    }
    
    public interface IDataKeyedListItem : IDataListItem
    {
        
    }


    internal class DataKeyedListItem : ContainerLikeDataNode<IResolvedSchemaList>, IDataKeyedListItem
    {
        public IDataKeyedList List { get; }

        public DataKeyedListItem(IDataKeyedList parent) : base(parent, parent.TypedSchemaNode)
        {
            this.List = parent;
        }
    }
    
    
    internal class DataKeyedList : DataNode<IResolvedSchemaList>, IDataKeyedList
    {

        public DataKeyedList(IDataObject parent, IResolvedSchemaList schemaNode) : base(parent, schemaNode)
        {
            this._Children = new (schemaNode);
        }

        public IReadOnlyList<QualifiedName> KeyNames => this._Children.KeyNames;
        private readonly KeyedListChildren _Children;
        public IReadOnlyKeyedCollection<ListKeySet, IDataKeyedListItem> Children => this._Children;
        public Result TryAddChild(IDataKeyedListItem child) => this._Children.TryAdd(child);

    }
}