using System.Collections.Generic;
using System.Linq;
using NetSchema.Common;
using NetSchema.Common.Collections;

#nullable enable

namespace NetSchema.Resolve.Nodes
{
    public interface IResolvedSchemaList : IResolvedSchemaDataNode
    {
        public bool HasKeys { get; }
        IReadOnlyKeyedCollection<IResolvedSchemaLeaf> Keys { get; }
        string? Description { get; }
        string? Reference { get; }
        OptionalValue<bool> Config { get; }
    }

    
    internal class ResolvedSchemaList : ResolvedSchemaDataNode, IResolvedSchemaList
    {
        public IReadOnlyKeyedCollection<IResolvedSchemaLeaf> Keys { get; }

        public ResolvedSchemaList(
            string name, 
            IEnumerable<IResolvedSchemaDataNode> children, 
            IEnumerable<IResolvedSchemaLeaf> keys
        ) : base(name, children)
        {
            this.Keys = new NamedNodeCollection<IResolvedSchemaLeaf>(keys).AsReadOnly();
        }

        public string? Description { get; init; }
        public string? Reference { get; init; }
        public OptionalValue<bool> Config { get; init; } = OptionalValue.CreateConfig();
        public override StatementType StatementType => StatementType.List;
        public bool HasKeys => Keys.Count > 0;
    }
}