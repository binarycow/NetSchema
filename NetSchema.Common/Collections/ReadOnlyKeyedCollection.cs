#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NetSchema.Common.Collections
{
    internal class ReadOnlyKeyedCollection<TValue>
        : ReadOnlyKeyedCollection<string, TValue>, IReadOnlyKeyedCollection<TValue>
        where TValue : IReadOnlyNamedNode
    {
        public static readonly IReadOnlyKeyedCollection<TValue> Empty
            = new EmptyCollection();
        
        public ReadOnlyKeyedCollection(IKeyedCollection<string, TValue> wrapped) : base(wrapped)
        {
        }

        public ReadOnlyKeyedCollection(GetKey<string, TValue> getKeyDelegate) : base(getKeyDelegate)
        {
        }

        private class EmptyCollection : IReadOnlyKeyedCollection<TValue>
        {
            public IEnumerator<TValue> GetEnumerator() => Enumerable.Empty<TValue>().GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
            public int Count => 0;
            public bool Contains(TValue item) => false;
            public bool Contains(string key) => false;
            public bool IsReadOnly => true;
            public TValue? this[string key] => default;
            public bool TryGetValue(string key, [NotNullWhen(true)] out TValue? item)
            {
                item = default;
                return false;
            }
            public TValue? GetValueOrDefault(string key) => default;
            TValue IReadOnlyList<TValue>.this[int index] => default!; // Null-forgiving
        }
    }

    internal class ReadOnlyKeyedCollection<TKey, TValue> : IReadOnlyKeyedCollection<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        public ReadOnlyKeyedCollection(IKeyedCollection<TKey, TValue> wrapped) => this.wrapped = wrapped;
        public ReadOnlyKeyedCollection(GetKey<TKey, TValue> getKeyDelegate) 
            : this(new DelegateKeyedCollection<TKey, TValue>(getKeyDelegate))
        {
            
        }
        private readonly IKeyedCollection<TKey, TValue> wrapped;
        public IEnumerator<TValue> GetEnumerator() => this.wrapped.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public int Count => this.wrapped.Count;
        public TValue? this[int index] => this.wrapped[index];
        public bool Contains(TValue item) => this.wrapped.Contains(item);
        public bool Contains(TKey key) => this.wrapped.Contains(key);

        public bool IsReadOnly => true;
        public TValue? this[TKey key] => this.wrapped[key];
        public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? item) => this.wrapped.TryGetValue(key, out item);
        public TValue? GetValueOrDefault(TKey key) => this.wrapped.GetValueOrDefault(key);

        TValue IReadOnlyList<TValue>.this[int index] => this[index]!; // Null-forgiving operator
    }
}