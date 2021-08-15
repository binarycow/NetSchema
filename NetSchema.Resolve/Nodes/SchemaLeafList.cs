using System.Collections.Generic;
using NetSchema.Common;
using NetSchema.Types;

#nullable enable

namespace NetSchema.Resolve.Nodes
{
    public interface IResolvedSchemaLeafList : IResolvedSchemaDataNode
    {
        IUsableType Type { get; }
        public OptionalValue<bool> Config { get; }
        string? Description { get; }
        string? Reference { get; }
    }
    
    internal class ResolvedSchemaLeafList : ResolvedSchemaDataNode, IResolvedSchemaLeafList
    {
        public ResolvedSchemaLeafList(string name, IUsableType type, IEnumerable<ResolvedSchemaDataNode> children) : base(name, children)
        {
            this.Type = type;
        }
        public ResolvedSchemaLeafList(string name, IUsableType type) : base(name)
        {
            this.Type = type;
        }
        public ResolvedSchemaLeafList(string name, IUsableType type, params ResolvedSchemaDataNode[] children) : base(name, children)
        {
            this.Type = type;
        }

        public IUsableType Type { get; }
        public OptionalValue<bool> Config { get; init; } = OptionalValue.CreateConfig();
        public string? Description { get; init; }
        public string? Reference { get; init; }
        public override StatementType StatementType => StatementType.LeafList;
    }
}