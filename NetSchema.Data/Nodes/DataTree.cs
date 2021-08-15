#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Common.Collections;
using NetSchema.Resolve.Nodes;

namespace NetSchema.Data.Nodes
{
    public interface IDataTree : IContainerLikeDataObject
    {
        public IResolvedSchema Schema { get; }
    }
    internal class DataTree : DataObject, IDataTree
    {
        public static IDataTree Create(IResolvedSchema schema) => new DataTree(schema);
        private DataTree(IResolvedSchema schema)
        {
            this.Schema = schema;
        }
        private readonly NsKeyedCollection<QualifiedName, IDataNode> _Children = new DataChildren();

        public Result TryAddChild(IDataNode child) => this._Children.TryAdd(child)
            ? Result.SuccessfulResult
            : Result.CreateError("Could not add item.");
        
        public IReadOnlyKeyedCollection<QualifiedName, IDataNode> Children => this._Children.AsReadOnly();
        public IResolvedSchema Schema { get; }
        public override IDataObject? Parent => null;
        public override IDataTree Tree => this;
        protected sealed override bool TryGetModuleNamespace(string moduleName, [NotNullWhen(true)] out XNamespace? namespaceName)
            => this.Schema.TryGetModuleNamespace(moduleName, out namespaceName);
        protected sealed override bool TryGetModuleName(XNamespace namespaceName, [NotNullWhen(true)] out string? moduleName) 
            => this.Schema.TryGetModuleName(namespaceName, out moduleName);
    }
}