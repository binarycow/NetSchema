#nullable enable

namespace NetSchema.Common.Collections
{
    internal delegate TKey GetKey<out TKey, in TValue>(TValue value);
    
    internal class DelegateKeyedCollection<TKey, TValue> : NsKeyedCollection<TKey, TValue>
        where TKey : notnull
        where TValue : notnull
    {
        private readonly GetKey<TKey, TValue> _GetKey;
        public DelegateKeyedCollection(GetKey<TKey, TValue> getKey) => this._GetKey = getKey;
        protected override TKey GetKeyForItem(TValue item) => this._GetKey(item);
    }
}