using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NetSchema.Restrictions
{
    public interface IReadOnlySchemaUnsignedRange : ITypeRestriction
    {
        IReadOnlyList<(UnsignedRangeArg Low, UnsignedRangeArg? High)> Ranges { get; }
    }
    public interface IReadOnlySchemaSignedRange : ITypeRestriction
    {
        IReadOnlyList<(SignedRangeArg Low, SignedRangeArg? High)> Ranges { get; }
    }
    
    internal static class RangeLength
    {
        public static IReadOnlyList<(UnsignedRangeArg Low, UnsignedRangeArg? High)> ParseUnsigned(string text)
            => ParseParts(text, TryParseUnsigned).ToList().AsReadOnly();
        public static IReadOnlyList<(SignedRangeArg Low, SignedRangeArg? High)> ParseSigned(string text) 
            => ParseParts(text, TryParseSigned).ToList().AsReadOnly();

        private static (SignedRangeArg Value, bool Success) TryParseSigned(string text) => text switch
        {
            "min" => (SignedRangeArg.Min(), true),
            "max" => (SignedRangeArg.Min(), true),
            { } when long.TryParse(text, out var value) => (SignedRangeArg.Value(value), true),
            _ => (default, false)
        };
        private static (UnsignedRangeArg Value, bool Success) TryParseUnsigned(string text) => text switch
        {
            "min" => (UnsignedRangeArg.Min(), true),
            "max" => (UnsignedRangeArg.Min(), true),
            { } when ulong.TryParse(text, out var value) => (UnsignedRangeArg.Value(value), true),
            _ => (default, false)
        };

        private static IEnumerable<(T Low, T? High)> ParseParts<T>(
            string text, 
            Func<string, (T Value, bool Success)> tryParse
        ) where T : struct
        {
            foreach (var (lowStr, highStr) in GetParts(text))
            {
                if(!TryParsePart(lowStr, tryParse, out var low))
                    throw new FormatException($"value '{text}' is not a valid range/length string");
                if(!TryParsePartOrNull(highStr, tryParse, out var high))
                    throw new FormatException($"value '{text}' is not a valid range/length string");
                yield return (low, high);
            }
        }

        private static bool TryParsePartOrNull<T>(
            string? part,
            Func<string, (T Value, bool Success)> tryParse,
            out T? result
        ) where T : struct
        {
            if (part is null || !TryParsePart(part, tryParse, out var res))
            {
                result = default;
                return false;
            }
            result = res;
            return true;
        }
        
        private static bool TryParsePart<T>(
            string part,
            Func<string, (T Value, bool Success)> tryParse,
            out T result
        ) where T : struct
        {
            bool success;
            (result, success) = tryParse(part);
            return success;
        }

        private static IEnumerable<(string Low, string? High)> GetParts(string text)
        {
            // TODO: make this better.
            var parts = text.Split('|');
            foreach (var part in parts)
            {
                var subParts = part.Split(new []{".."}, StringSplitOptions.None);
                if (subParts.Length is 0 or > 2)
                    throw new FormatException($"value '{text}' is not a valid range/length string");
                var high = subParts.Length == 2 ? subParts[1] : null;
                yield return (subParts[0], high);
            }
        }


        public static string ToRangeString(this IEnumerable<(UnsignedRangeArg Low, UnsignedRangeArg? High)> ranges) 
            => string.Join("|", ranges.Select(ToRangeString));

        public static string ToRangeString(this IEnumerable<(SignedRangeArg Low, SignedRangeArg? High)> ranges)
            => string.Join("|", ranges.Select(ToRangeString));
        private static string ToRangeString(string low, string? high) 
            => high is null ? low : $"{low}..{high}";
        private static string ToRangeString((UnsignedRangeArg Low, UnsignedRangeArg? High) arg)
            => ToRangeString(arg.Low.ToString(), arg.High?.ToString());
        private static string ToRangeString((SignedRangeArg Low, SignedRangeArg? High) arg)
            => ToRangeString(arg.Low.ToString(), arg.High?.ToString());
    }
}