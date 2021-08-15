using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;

#nullable enable

namespace NetSchema.Common
{
    public readonly struct QualifiedName : IEquatable<QualifiedName>
    {
        public string ModuleName { get; }
        public string LocalName { get; }

        public QualifiedName(string moduleName, string localName)
        {
            this.ModuleName = moduleName;
            this.LocalName = localName;
        }

        public bool Equals(QualifiedName other) => this.ModuleName == other.ModuleName && this.LocalName == other.LocalName;
        public override bool Equals(object? obj) => obj is QualifiedName other && this.Equals(other);
        public override int GetHashCode() => HashCode.Combine(this.ModuleName, this.LocalName);
        public static bool operator ==(QualifiedName left, QualifiedName right) => left.Equals(right);
        public static bool operator !=(QualifiedName left, QualifiedName right) => !left.Equals(right);

        public override string ToString() => $"{this.ModuleName}:{this.LocalName}";

        public XName ToXName(IModuleNameResolver resolver)
        {
            var ns = resolver.GetModuleNamespace(this.ModuleName) ?? XNamespace.None;
            return ns + this.LocalName;
        }

        public static XName ToXName(QualifiedName qName, IModuleNameResolver resolver) => qName.ToXName(resolver);
        public static QualifiedName FromXName(XName xName, IModuleNameResolver resolver)
        {
            var moduleName = resolver.GetModuleName(xName.NamespaceName) ?? string.Empty;
            return new(moduleName, xName.LocalName);
        }

        public static QualifiedName Parse(string name) 
            => TryParse(name, out var qName)
                ? qName 
                : throw new FormatException($"'{name}' is not a valid qualified name.");
        
        public static bool TryParse(string name, out QualifiedName qName)
        {
            var index = 0;
            return TryParse(name, ref index, out qName) && index == name.Length;
        }

        internal static bool TryParse(string name, ref int index, out QualifiedName qName)
        {
            qName = default;
            var indexCopy = index;
            if (!TryParseIdentifier(name, ref indexCopy, out var first))
            {
                return false;
            }
            if (indexCopy >= name.Length || name[indexCopy] != ':')
            {
                index = indexCopy;
                qName = new (string.Empty, first);
                return true;
            }
            ++indexCopy;
            if (indexCopy >= name.Length)
            {
                return false;
            }
            if (!TryParseIdentifier(name, ref indexCopy, out var second))
            {
                return false;
            }
            index = indexCopy;
            qName = new (first, second);
            return true;
        }

        private static bool TryParseIdentifier(string name, ref int index, [NotNullWhen(true)] out string? identifier)
        {
            var first = true;
            var start = index;
            for (; index < name.Length; ++index)
            {
                var isValid = first ? IsStartChar(name[index]) : IsRemainderChar(name[index]);
                if (!isValid)
                {
                    break;
                }
                first = false;
            }
            var length = index - start;
            identifier = length switch
            {
                < 0 => throw new InvalidOperationException(GetExceptionMessage(length)),
                { } when start + length > name.Length => throw new InvalidOperationException(GetExceptionMessage(length)),
                0 => null,
                >= 3 when QualifiedName.CheckXml(name, start) == false => null,
                _ => name.Substring(start, length)
            };
            return identifier is not null;

            static string GetExceptionMessage(int length) => $"Somehow got invalid length {length.ToString()} in {nameof(QualifiedName)}.{nameof(QualifiedName.TryParseIdentifier)}";
        }

        private static bool CheckXml(string name, int start)
        {
            // An identifier MUST NOT start with (('X'|'x') ('M'|'m') ('L'|'l'))
            // RFC 6020, Section 12
            return name.Length - start < 3 
                   || name[start] != 'X' && name[start] != 'x'
                   || name[start + 1] != 'M' && name[start] != 'm'
                   || name[start + 2] != 'L' && name[start] != 'l';
        }

        private static bool IsStartChar(char c)
        {
            return c switch
            {
                >= 'A' and <= 'Z' => true,
                >= 'a' and <= 'z' => true,
                '_' => true,
                _ => false
            };
        }
        private static bool IsRemainderChar(char c)
        {
            return c switch
            {
                >= 'A' and <= 'Z' => true,
                >= 'a' and <= 'z' => true,
                >= '0' and <= '9' => true,
                '_' => true,
                '-' => true,
                '.' => true,
                _ => false
            };
        }

    }
}