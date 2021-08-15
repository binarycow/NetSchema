using NetSchema.Common;
using NetSchema.Common.Collections;

namespace NetSchema.Resolve.Nodes
{
    public interface IResolvedSchema : IModuleNameResolver
    {
        IReadOnlyKeyedCollection<IResolvedSchemaModule> Modules { get; }
    }
}