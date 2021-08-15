using System.Text.RegularExpressions;
using NetSchema.Common;

#nullable enable

namespace NetSchema.Restrictions
{
    public interface IReadOnlySchemaPattern : ITypeRestriction
    {
        Regex Pattern { get; }
        string? ErrorMessage { get; }
        string? ErrorAppTag { get; }
        string? Description { get; }
        string? Reference { get; }
        OptionalValue<PatternModifier> Modifier { get; }
    }

    public class ReadOnlySchemaPattern : IReadOnlySchemaPattern
    {
        public ReadOnlySchemaPattern(Regex pattern)
        {
            this.Pattern = pattern;
        }
        public Regex Pattern { get; }
        public string? ErrorMessage { get; init; }
        public string? ErrorAppTag { get; init; }
        public string? Description { get; init; }
        public string? Reference { get; init; }
        public OptionalValue<PatternModifier> Modifier { get; init; } = OptionalValue.CreatePatternModifier();
        public Result Validate(string value)
        {
            var isMatch = Pattern.IsMatch(value);
            var success = Modifier.EffectiveValue == PatternModifier.None
                ? isMatch
                : !isMatch;
            return success
                ? Result.SuccessfulResult
                : Result.CreateError(ErrorMessage, ErrorAppTag);
        }
    }
}