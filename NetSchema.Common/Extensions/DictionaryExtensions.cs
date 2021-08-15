
// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    internal static class DictionaryExtensions
    {
        public static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            => dictionary.TryGetValue(key ?? throw new ArgumentNullException(nameof(key)), out var value) ? value : default;
    }
}