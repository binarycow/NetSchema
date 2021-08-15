using System;
using System.Collections.Generic;

namespace NetSchema.Common
{
    public readonly struct OptionalValue<T> : IEquatable<OptionalValue<T>> where T : struct
    {
        public static IEqualityComparer<OptionalValue<T>> EffectiveValueComparer { get; } = new EffectiveValueEqualityComparer();
        
        internal OptionalValue(T? userValue, T defaultValue)
        {
            this.UserValue = userValue;
            this.DefaultValue = defaultValue;
            this.EffectiveValue = UserValue ?? DefaultValue;
        }
        public T? UserValue { get; }
        public T DefaultValue { get; }
        public T EffectiveValue { get; }
        
        public bool Equals(OptionalValue<T> other) => Nullable.Equals(this.UserValue, other.UserValue) && this.DefaultValue.Equals(other.DefaultValue);
        public override bool Equals(object? obj) => obj is OptionalValue<T> other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(this.UserValue, this.DefaultValue);
        public static bool operator ==(OptionalValue<T> left, OptionalValue<T> right) => left.Equals(right);
        public static bool operator !=(OptionalValue<T> left, OptionalValue<T> right) => !left.Equals(right);

        
        private sealed class EffectiveValueEqualityComparer : IEqualityComparer<OptionalValue<T>>
        {
            public bool Equals(OptionalValue<T> x, OptionalValue<T> y)
            {
                return x.EffectiveValue.Equals(y.EffectiveValue);
            }

            public int GetHashCode(OptionalValue<T> obj)
            {
                return obj.EffectiveValue.GetHashCode();
            }
        }
    }

    public static class OptionalValue
    {
        
        public static void SetValue<T>(ref this OptionalValue<T> optional, T? value)
            where T : struct
            => optional = optional.WithValue(value);

        public static OptionalValue<T> WithValue<T>(this OptionalValue<T> optional, T? value)
            where T : struct
            => new(value, optional.DefaultValue);
        
        private static bool? ParseBool(string? argument) => argument switch
        {
            null => null,
            "true" => true,
            "false" => false,
            _ => throw new ArgumentOutOfRangeException(nameof(argument)),
        };
        
        private static PatternModifier? ParsePatternModifier(string? argument) => argument switch
        {
            null => null,
            "invert-match" => PatternModifier.InvertMatch,
            _ => throw new ArgumentOutOfRangeException(nameof(argument)),
        };
        
        private static YangVersion? ParseVersion(string? argument) => argument switch
        {
            null => null,
            "1" => YangVersion.Rfc6020,
            "1.1" => YangVersion.Rfc7950,
            _ => throw new ArgumentOutOfRangeException(nameof(argument)),
        };

        public static OptionalValue<YangVersion> CreateVersion(YangVersion? version = null)
            => new (version, YangVersion.Rfc6020);
        public static OptionalValue<YangVersion> CreateVersion(string? argument) 
            => CreateVersion(ParseVersion(argument));
        
        public static OptionalValue<bool> CreateConfig(bool? value = null) 
            => new (value, true);
        public static OptionalValue<bool> CreateConfig(string? argument)
            => CreateConfig(ParseBool(argument));

        public static OptionalValue<bool> CreatePresence(bool? value = null) 
            => new (value, false);
        public static OptionalValue<bool> CreatePresence(string? value)
            => CreatePresence(ParseBool(value));

        public static OptionalValue<PatternModifier> CreatePatternModifier(PatternModifier? value = null)
            => new (value, PatternModifier.None);
        public static OptionalValue<PatternModifier> CreatePatternModifier(string? value)
            => CreatePatternModifier(ParsePatternModifier(value));
    }
}