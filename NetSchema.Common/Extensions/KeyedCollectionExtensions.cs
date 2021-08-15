#nullable enable

using System;
using NetSchema.Common;
using NetSchema.Common.Collections;
using NetSchema.Data;
using NetSchema.Data.Collections;

// ReSharper disable once CheckNamespace
namespace NetSchema.Data.Collections
{
    internal static class KeyedCollectionExtensions
    {
        public static Result<TValue> TryGetValue<TKey, TValue>(
            this IReadOnlyKeyedCollection<TKey, TValue> collection,
            TKey key
        )
            where TKey : notnull
            where TValue : notnull 
            => collection.TryGetValue(key, out var value) ? value : Result<TValue>.CreateError();
    }
}