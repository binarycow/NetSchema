using NetSchema.Common;
using NetSchema.Common.Collections;

#nullable enable

namespace NetSchema.Resolve.Nodes
{
    
    public interface IResolvedSchemaNode
    {
        public StatementType StatementType { get; }
        public string Argument { get; }
    }

    public interface IResolvedNamedSchemaNode : IResolvedSchemaNode, IReadOnlyNamedNode
    {
        public QualifiedName QualifiedName { get; }
    }

    internal abstract class ResolvedNamedSchemaNode : ResolvedSchemaNode, IResolvedNamedSchemaNode
    {
        protected ResolvedNamedSchemaNode(string name) => this.Name = name;
        public QualifiedName QualifiedName => new (this.Module?.Name ?? string.Empty, this.Name);
        public string Name { get; }
        public sealed override string Argument => this.Name;
    }
    
    
    internal abstract class ResolvedSchemaNode : IResolvedSchemaNode, IParent<ResolvedSchemaNode>
    {
        public abstract StatementType StatementType { get; }
        public abstract string Argument { get; }
        internal ResolvedSchemaNode? Parent { get; set; }
        internal ResolvedSchemaModule? Module => this is ResolvedSchemaModule mod ? mod : this.Parent?.Module;

        ResolvedSchemaNode? IParent<ResolvedSchemaNode>.Parent
        {
            set => this.Parent = value;
        }
    }


}