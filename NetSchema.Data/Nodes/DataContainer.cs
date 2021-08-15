#nullable enable

using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Common.Collections;
using NetSchema.Resolve.Nodes;

namespace NetSchema.Data.Nodes
{
    public interface IContainerLikeDataObject : IDataObject
    {
        IReadOnlyKeyedCollection<QualifiedName, IDataNode> Children { get; }
        Result TryAddChild(IDataNode child);
    }

    public interface IContainerLikeDataNode : IContainerLikeDataObject, IDataNode
    {
        
    }

    internal abstract class ContainerLikeDataNode<T> : DataNode<T>
        where T : class, IResolvedSchemaDataNode
    {
        protected ContainerLikeDataNode(IDataObject parent, T schemaNode) : base(parent, schemaNode)
        {
        }
        
        private readonly NsKeyedCollection<QualifiedName, IDataNode> _Children = new DataChildren();
        public Result TryAddChild(IDataNode child) => this._Children.TryAdd(child) ? Result.SuccessfulResult : Result.CreateError("Could not add item.");
        public IReadOnlyKeyedCollection<QualifiedName, IDataNode> Children => this._Children.AsReadOnly();
    }
    
    public interface IDataContainer : IContainerLikeDataNode
    {
    }
    
    internal class DataContainer : ContainerLikeDataNode<IResolvedSchemaContainer>, IDataContainer
    {
        internal DataContainer(IDataObject parent, IResolvedSchemaContainer schemaNode) : base(parent, schemaNode)
        {
        }
    }
}