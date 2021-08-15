#nullable enable


// ReSharper disable once CheckNamespace
namespace System.Collections.Generic
{
    internal static class EnumerableExtensions
    {
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items) => new (items);

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> items)
        {
            foreach (var item in items)
            {
                if (item is not null)
                    yield return item;
            }
        }
    }
}