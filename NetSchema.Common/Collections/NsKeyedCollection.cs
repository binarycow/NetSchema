#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace NetSchema.Common.Collections
{
    internal abstract class NsKeyedCollection<TKey, TValue> : KeyedCollection<TKey, TValue>, IKeyedCollection<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        protected NsKeyedCollection()
        {
            
        }
        protected NsKeyedCollection(IEqualityComparer<TKey> comparer) : base(comparer)
        {
            
        }
        public IReadOnlyKeyedCollection<TKey, TValue> AsReadOnly() => new ReadOnlyKeyedCollection<TKey, TValue>(this);
        public virtual bool IsReadOnly => false;
        public bool TryGetValue(TKey key, [NotNullWhen(true)] out TValue? item)
        {
            item = default;
            if (this.Contains(key))
            {
                item = this[key];
            }
            return item is not null;
        }
        public TValue? GetValueOrDefault(TKey key)
        {
            _ = this.TryGetValue(key, out var item);
            return item;
        }
        public Result TryAdd(TValue item)
        {
            if (this.Contains(item))
            {
                return new DuplicateNameException();
            }
            try
            {
                this.Add(item);
                return Result.SuccessfulResult;
            }
            catch(Exception ex)
            {
                return ex;
            }
        }

        public void AddRange(IEnumerable<TValue> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }
    }
}