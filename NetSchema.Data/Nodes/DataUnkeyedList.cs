using System.Collections.Generic;
using NetSchema.Common;
using NetSchema.Resolve.Nodes;

namespace NetSchema.Data.Nodes
{
    public interface IDataList : IDataNode<IResolvedSchemaList>
    {
        
    }
    
    public interface IDataUnkeyedList : IDataList
    {
        IReadOnlyList<IDataUnkeyedListItem> Children { get; }
        Result TryAddChild(IDataUnkeyedListItem child);
    }

    public interface IDataUnkeyedListItem : IDataListItem
    {
        
    }

    public interface IDataListItem : IContainerLikeDataNode
    {
        
    }
    internal class DataUnkeyedListItem : ContainerLikeDataNode<IResolvedSchemaList>, IDataUnkeyedListItem
    {
        public IDataUnkeyedList List { get; }

        public DataUnkeyedListItem(IDataUnkeyedList parent) : base(parent, parent.TypedSchemaNode)
        {
            this.List = parent;
        }
    }

    internal class DataUnkeyedList : DataNode<IResolvedSchemaList>, IDataUnkeyedList
    {
        public DataUnkeyedList(IDataObject parent, IResolvedSchemaList schemaNode) : base(parent, schemaNode)
        {
        }

        private readonly List<IDataUnkeyedListItem> _Children = new();
        public IReadOnlyList<IDataUnkeyedListItem> Children => _Children;
        public Result TryAddChild(IDataUnkeyedListItem child)
        {
            this._Children.Add(child);
            return Result.SuccessfulResult;
        }
    }
}