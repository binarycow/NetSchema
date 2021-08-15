using System.Collections.Generic;
using System.Linq;
using NetSchema.Common;
using NetSchema.Common.Collections;

#nullable enable

namespace NetSchema.Resolve.Nodes
{
    public interface IResolvedSchemaDataNode : IResolvedNamedSchemaNode
    {
        public IReadOnlyKeyedCollection<IResolvedSchemaDataNode> DataNodes { get; }
    }
    
    internal abstract class ResolvedSchemaDataNode : ResolvedNamedSchemaNode, IResolvedSchemaDataNode
    {
        protected ResolvedSchemaDataNode(string name, IEnumerable<IResolvedSchemaDataNode> children) : base(name)
        {
            this.DataNodes = ReadOnlyCollectionFactory.CreateReadOnly(children, this);
        }

        protected ResolvedSchemaDataNode(string name) : this(name, Enumerable.Empty<IResolvedSchemaDataNode>())
        {
        }
        protected ResolvedSchemaDataNode(string name, params IResolvedSchemaDataNode[] children) : this(name, (IEnumerable<IResolvedSchemaDataNode>) children)
        {
        }

        public IReadOnlyKeyedCollection<IResolvedSchemaDataNode> DataNodes { get; }
    }
}