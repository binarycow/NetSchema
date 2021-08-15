#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Common.Collections;
using NetSchema.Resolve.Nodes;

namespace NetSchema.Data.Nodes
{
    public interface IDataNode : IDataObject
    {
        IResolvedSchemaDataNode SchemaNode { get; }
        QualifiedName Name { get; }
    }

    public interface IDataNode<out T> : IDataNode where T : IResolvedSchemaDataNode
    {
        public T TypedSchemaNode { get; }
    }
    
    internal class DataChildren : NsKeyedCollection<QualifiedName, IDataNode>
    {
        protected override QualifiedName GetKeyForItem(IDataNode item) => item.Name;
    }

    internal abstract class DataNode<T> : DataNode, IDataNode<T>
        where T : class, IResolvedSchemaDataNode
    {
        protected DataNode(IDataObject parent, T schemaNode) : base(parent, schemaNode)
        {
            this.TypedSchemaNode = schemaNode;
        }

        public T TypedSchemaNode { get; }
    }
    
    
    internal abstract class DataNode : DataObject, IDataNode
    {
        protected DataNode(IDataObject parent, IResolvedSchemaDataNode schemaNode)
        {
            this.SchemaNode = schemaNode;
            this.Parent = parent;
            this.Name = schemaNode.QualifiedName;
        }

        public QualifiedName Name { get; }
        public override IDataObject Parent { get; }
        public override IDataTree Tree => this.Parent.Tree;

        public IResolvedSchemaDataNode SchemaNode { get; }

        protected sealed override bool TryGetModuleNamespace(string moduleName, [NotNullWhen(true)] out XNamespace? namespaceName)
            => this.Tree.TryGetModuleNamespace(moduleName, out namespaceName);
        protected sealed override bool TryGetModuleName(XNamespace namespaceName, [NotNullWhen(true)] out string? moduleName) 
            => this.Tree.TryGetModuleName(namespaceName, out moduleName);
    }


    
}