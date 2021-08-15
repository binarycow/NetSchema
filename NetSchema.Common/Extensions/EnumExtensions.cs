using System;
using System.ComponentModel;
using System.Reflection;

namespace NetSchema.Common.Extensions
{
    internal static class EnumExtensions
    {
        public static string GetDescription<T>(this T source) where T : struct, Enum
        {
            var textValue = source.ToString();
            if (textValue is null) 
                return $"Unknown: {typeof(T).Name}";
            var attribute = source.GetType().GetField(textValue)?.GetCustomAttribute<DescriptionAttribute>(false)?.Description;
            return attribute.IsNullOrWhiteSpace() 
                ? textValue 
                : attribute;
        }
    }
}