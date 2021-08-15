using System;
using System.Diagnostics.CodeAnalysis;
using NetSchema.Common;

// ReSharper disable once CheckNamespace
namespace NetSchema
{
    public static class YangValues
    {
        public static string? ToYangValue(this object? value) => value switch
        {
            null => null,
            bool v => v.ToYangValue(),
            YangVersion v => v.ToYangValue(),
            _ => value.ToString(),
        };
        
        [return: NotNullIfNotNull("value")]
        public static string? ToYangValue(this YangVersion? value) => value switch
        {
            YangVersion.Rfc6020 => "1",
            YangVersion.Rfc7950 => "1.1",
            null => null,
            _ => throw new ArgumentOutOfRangeException(nameof(value)),
        };
        [return: NotNullIfNotNull("value")]
        public static string? ToYangValue(this bool? value) => value switch
        {
            true => "true",
            false => "false",
            null => null,
        };
    }
}