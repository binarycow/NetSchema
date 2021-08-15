using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace System
{
    internal static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace(
            [NotNullWhen(false)] this string? str
        ) => str is null || string.IsNullOrWhiteSpace(str);
        
        public static bool IsNotNullOrWhiteSpace(
            [NotNullWhen(true)] this string? str
        ) => str is not null && string.IsNullOrWhiteSpace(str) == false;

        
        [return: NotNullIfNotNull("str")]
        public static string? NullIfNullOrWhiteSpace(this string? str) => str.IsNullOrWhiteSpace() ? null : str;
    }
}