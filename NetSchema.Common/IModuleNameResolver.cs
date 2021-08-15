using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

#nullable enable

namespace NetSchema.Common
{
    public interface IModuleNameResolver
    {
        public bool TryGetModuleNamespace(string moduleName, [NotNullWhen(true)] out XNamespace? namespaceName);
        public bool TryGetModuleName(XNamespace namespaceName, [NotNullWhen(true)] out string? moduleName);
    }

    public static class ModuleResolverExtensions
    {

        public static XNamespace? GetModuleNamespace(this IModuleNameResolver resolver, string moduleName)
            => resolver.TryGetModuleNamespace(moduleName, out var ns) ? ns : null;
        public static string? GetModuleName(this IModuleNameResolver resolver, XNamespace namespaceName)
            => resolver.TryGetModuleName(namespaceName, out var name) ? name : null;
    }
}