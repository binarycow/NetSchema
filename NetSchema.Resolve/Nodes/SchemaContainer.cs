using System.Collections.Generic;
using NetSchema.Common;
using NetSchema.Common.Collections;

#nullable enable

namespace NetSchema.Resolve.Nodes
{
    public interface IResolvedSchemaContainer : IResolvedSchemaDataNode
    {        
        public OptionalValue<bool> Config { get; }
        public OptionalValue<bool> Presence { get; }
    }

    internal class ResolvedSchemaContainer : ResolvedSchemaDataNode, IResolvedSchemaContainer
    {
        public ResolvedSchemaContainer(string name, IEnumerable<IResolvedSchemaDataNode> children) : base(name, children)
        {
        }
        public ResolvedSchemaContainer(string name) : base(name)
        {
        }
        public ResolvedSchemaContainer(string name, params IResolvedSchemaDataNode[] children) : base(name, children)
        {
        }

        public override StatementType StatementType => StatementType.Container;
        public OptionalValue<bool> Config { get; init; } = OptionalValue.CreateConfig();
        public OptionalValue<bool> Presence { get; init;} = OptionalValue.CreatePresence();
    }
}