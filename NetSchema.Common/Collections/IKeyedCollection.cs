using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace NetSchema.Common.Collections
{
    public interface IReadOnlyKeyedCollection<in TKey, TValue> : IReadOnlyList<TValue>
        where TKey : notnull
        where TValue : notnull
    {
        public bool Contains(TValue item);
        public bool Contains(TKey key);
        public bool IsReadOnly { get; }
        public TValue? this[TKey key] { get; }
        public bool TryGetValue(TKey key, [NotNullWhen(true)]  out TValue? item);
        public TValue? GetValueOrDefault(TKey key);
    }

    public interface IReadOnlyKeyedCollection<TValue> : IReadOnlyKeyedCollection<string, TValue>
        where TValue : IReadOnlyNamedNode
    {
        
    }
    
    public interface IKeyedCollection<in TKey, TValue> : IReadOnlyKeyedCollection<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        public void Clear();
        public Result TryAdd(TValue item);
        public void Add(TValue item);
        public bool Remove(TValue item);
        public void AddRange(IEnumerable<TValue> items);
    }
}