using NetSchema.Common;
using NetSchema.Types;

namespace NetSchema.Resolve.Nodes
{
    public interface IResolvedSchemaTypedef : IResolvedNamedSchemaNode
    {
        IUsableType Type { get; }
    }
    
    internal class ResolvedSchemaTypedef : ResolvedNamedSchemaNode, IResolvedSchemaTypedef
    {
        public IUsableType Type { get; }

        public ResolvedSchemaTypedef(string name, IUsableType type) : base(name)
        {
            this.Type = type;
        }

        public override StatementType StatementType => StatementType.Typedef;
    }
}