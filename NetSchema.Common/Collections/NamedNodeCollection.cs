#nullable enable

using System.Collections.Generic;
using System.Linq;

namespace NetSchema.Common.Collections
{
    internal class NamedNodeCollection<T> : NsKeyedCollection<string, T>
        where T : IReadOnlyNamedNode
    {
        public NamedNodeCollection(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        public NamedNodeCollection() : this(Enumerable.Empty<T>())
        {
            
        }
        public NamedNodeCollection(params T[] items) : this((IEnumerable<T>)items)
        {
            
        }
        public new IReadOnlyKeyedCollection<T> AsReadOnly() => new ReadOnlyKeyedCollection<T>(this);
        
        protected override string GetKeyForItem(T item) => item.Name;
        public sealed override bool IsReadOnly => false;
    }
    
    internal static class ReadOnlyCollectionFactory
    {
        public static IReadOnlyKeyedCollection<T> CreateReadOnly<T, TParent>(IEnumerable<T> items, TParent? parent = default)
            where T : IReadOnlyNamedNode 
            => new ReadOnlyKeyedCollection<T>(new NamedNodeCollection<T>(SetParent(items, parent)));

        private static IEnumerable<TItem> SetParent<TItem, TParent>(IEnumerable<TItem> items, TParent parent)
        {
            foreach (var item in items)
            {
                if (item is IParent<TParent> parentAware)
                {
                    parentAware.Parent = parent;
                }
                yield return item;
            }
        }
    }
}