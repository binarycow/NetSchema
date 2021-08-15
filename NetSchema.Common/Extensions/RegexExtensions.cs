// ReSharper disable once CheckNamespace
namespace System.Text.RegularExpressions
{
    internal static class RegexExtensions
    {
        public static bool TryMatch(this Regex regex, string input, out Match match)
        {
            match = regex.Match(input);
            return match.Success;
        }
    }
}