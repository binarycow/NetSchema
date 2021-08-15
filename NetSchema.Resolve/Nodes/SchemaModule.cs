using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Common.Collections;

#nullable enable

namespace NetSchema.Resolve.Nodes
{
    public interface IResolvedSchemaModule : IResolvedNamedSchemaNode, IReadOnlyNamedNode
    {
        string Prefix { get; }
        XNamespace Namespace { get; }
        public IReadOnlyKeyedCollection<string, IResolvedSchemaDataNode> DataNodes { get; }
        
        public XName XName { get; }
        public OptionalValue<YangVersion> Version { get; }
    }

    internal class ResolvedSchemaModule : ResolvedSchemaNode, IResolvedSchemaModule
    {
        public ResolvedSchemaModule(XName name, string prefix, IEnumerable<IResolvedSchemaDataNode> children)
        {
            this.Prefix = prefix;
            this.XName = name;
            this.DataNodes = ReadOnlyCollectionFactory.CreateReadOnly(children, this);
        }

        public string Prefix { get; }
        public XNamespace Namespace => XName.Namespace;
        public string Name => XName.LocalName;
        public override StatementType StatementType => StatementType.Module;
        public override string Argument => this.Name;
        public IReadOnlyKeyedCollection<string, IResolvedSchemaDataNode> DataNodes { get; }
        public XName XName { get; }
        public OptionalValue<YangVersion> Version { get; init; } = OptionalValue.CreateVersion();
        public QualifiedName QualifiedName => new (string.Empty, Name);
    }
}