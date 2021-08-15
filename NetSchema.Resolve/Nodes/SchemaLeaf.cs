using NetSchema.Common;
using NetSchema.Types;

#nullable enable

namespace NetSchema.Resolve.Nodes
{
    public interface IResolvedSchemaLeaf : IResolvedSchemaDataNode
    {
        IUsableType Type { get; }
        OptionalValue<bool> Config { get; }
        string? Description { get; }
        string? Reference { get; }
    }
    
    internal class ResolvedSchemaLeaf : ResolvedSchemaDataNode, IResolvedSchemaLeaf
    {
        public IUsableType Type { get; }
        public OptionalValue<bool> Config { get; init; } = OptionalValue.CreateConfig();
        public string? Description { get; init; }
        public string? Reference { get; init; }

        public ResolvedSchemaLeaf(string name, IUsableType type) : base(name)
        {
            this.Type = type;
        }
        public override StatementType StatementType => StatementType.Leaf;
    }
}