using System;
using System.Collections.Generic;

namespace NetSchema.Common
{
    public struct PrefixedName
    {
        public string? Prefix { get; }
        public string Name { get; }

        public PrefixedName(string? prefix, string name)
        {
            this.Prefix = prefix;
            this.Name = name;
        }

        public override string ToString() => Prefix is null ? Name : $"{Prefix}:{Name}";

        public static bool TryParse(string? text, out PrefixedName name)
        {
            name = default;
            if (text is null)
                return false;
            var index = 0;
            return TryParse(text, ref index, out name) && index == text.Length;
        }

        internal static bool TryParse(string text, ref int index, out PrefixedName name)
        {
            name = default;
            if (!QualifiedName.TryParse(text, ref index, out var qName))
                return false;
            name = qName.ModuleName.IsNullOrWhiteSpace() 
                ? new (null, qName.LocalName) 
                : new PrefixedName(qName.ModuleName, qName.LocalName);
            return true;
        }
        public static IReadOnlyList<PrefixedName> ParseList(string text)
        {
            if (text.Length == 0)
                return Array.Empty<PrefixedName>();
            var index = 0;
            List<PrefixedName>? items = null;
            while (index < text.Length && TryParse(text, ref index, out var item))
            {
                items ??= new ();
                items.Add(item);
                if (index == text.Length) 
                    return items.AsReadOnly();
                if (text[index] != ' ')
                    throw new FormatException($"'{text}' is not a valid space separated prefixed name list.");
                while (index < text.Length && text[index] == ' ')
                    ++index;
            }
            return items?.AsReadOnly() ?? (IReadOnlyList<PrefixedName>)Array.Empty<PrefixedName>();
        }
    }
}