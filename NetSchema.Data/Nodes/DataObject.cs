#nullable enable

using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using NetSchema.Common;

namespace NetSchema.Data.Nodes
{
    public interface IDataObject : IModuleNameResolver
    {
        IDataObject? Parent { get; }
        IDataTree Tree { get; }
    }

    public static class DataObjectExtensions
    {
        internal static Result TryAddChildExtension(this IDataObject parent, IDataNode child)
        {
            return (parent, child) switch
            {
                (IContainerLikeDataObject typedParent, _) 
                    => typedParent.TryAddChild(child),
                (IDataKeyedList typedParent, IDataKeyedListItem typedChild) 
                    => typedParent.TryAddChild(typedChild),
                (IDataKeyedList typedParent, _) 
                    => Result.CreateError($"Children of {nameof(IDataKeyedList)} must be a {nameof(IDataKeyedListItem)}"),
                (IDataUnkeyedList typedParent, IDataUnkeyedListItem typedChild) 
                    => typedParent.TryAddChild(typedChild),
                (IDataUnkeyedList typedParent, _) 
                    => Result.CreateError($"Children of {nameof(IDataUnkeyedList)} must be a {nameof(IDataUnkeyedListItem)}"),
                _ => throw new NotImplementedException()
            };
        }
    }
    
    internal abstract class DataObject : IDataObject
    {

        protected DataObject()
        {
        }
        public abstract IDataObject? Parent { get; }
        public abstract IDataTree Tree { get; }


        protected abstract bool TryGetModuleNamespace(string moduleName, [NotNullWhen(true)] out XNamespace? namespaceName);
        protected abstract bool TryGetModuleName(XNamespace namespaceName, [NotNullWhen(true)] out string? moduleName);
        
        bool IModuleNameResolver.TryGetModuleNamespace(string moduleName, [NotNullWhen(true)] out XNamespace? namespaceName) 
            => this.TryGetModuleNamespace(moduleName, out namespaceName);
        bool IModuleNameResolver.TryGetModuleName(XNamespace namespaceName, [NotNullWhen(true)] out string? moduleName)
            => this.TryGetModuleName(namespaceName, out moduleName);
    }
}