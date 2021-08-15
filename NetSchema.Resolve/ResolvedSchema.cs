using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Common.Collections;
using NetSchema.Resolve.Nodes;

namespace NetSchema.Resolve
{
    internal class ResolvedSchema : IResolvedSchema
    {
        public ResolvedSchema(IEnumerable<IResolvedSchemaModule> modules) 
            => this.Modules = ReadOnlyCollectionFactory.CreateReadOnly(modules, this);
        public IReadOnlyKeyedCollection<IResolvedSchemaModule> Modules { get; }

        bool IModuleNameResolver.TryGetModuleNamespace(
            string moduleName, 
            [NotNullWhen(true)] out XNamespace? namespaceName
        )
        {
            namespaceName = default;
            if (Modules.TryGetValue(moduleName, out var module))
                namespaceName = module.Namespace;
            return namespaceName is not null;
        }

        bool IModuleNameResolver.TryGetModuleName(
            XNamespace namespaceName, 
            [NotNullWhen(true)] out string? moduleName
        )
        {
            moduleName = Modules.FirstOrDefault(m => m.Namespace == namespaceName)?.Name;
            return moduleName is not null;
        }
    }
}