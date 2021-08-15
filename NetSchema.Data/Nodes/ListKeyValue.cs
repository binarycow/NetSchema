using System;
using System.Collections.Generic;
using System.Linq;
using NetSchema.Common;
using NetSchema.Types;

#nullable enable

namespace NetSchema.Data.Nodes
{
    public readonly struct ListKeyValue : IEquatable<ListKeyValue>
    {
        public QualifiedName LeafName { get; }
        public string Value { get; }

        public ListKeyValue(QualifiedName leafName, string value)
        {
            this.LeafName = leafName;
            this.Value = value;
        }

        public bool Equals(ListKeyValue other) => this.LeafName.Equals(other.LeafName) && this.Value == other.Value;
        public override bool Equals(object? obj) => obj is ListKeyValue other && this.Equals(other);
        public override int GetHashCode() => HashCode.Combine(this.LeafName, this.Value);
        public static bool operator ==(ListKeyValue left, ListKeyValue right) => left.Equals(right);
        public static bool operator !=(ListKeyValue left, ListKeyValue right) => !left.Equals(right);
    }

    public sealed class ListKeySet
    {
        private ListKeySet(string key)
        {
            this._Key = key;
            this._Keys = null;
        }
        private ListKeySet(string[] keys)
        {
            this._Key = string.Empty;
            this._Keys = keys;
        }

        private readonly string _Key;
        private readonly string[]? _Keys;
        public int KeyCount => this._Keys?.Length ?? 1;

        private string this[int index] => this.GetKeyValue(index);

        private string GetKeyValue(int index)
        {
            if (index == 0 && this._Keys is null)
                return this._Key;
            if (this._Keys is null)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (index < 0 || index >= this._Keys.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            return this._Keys[index];
        }
        
        public static ListKeySet Create(IEnumerable<(QualifiedName Name, object? Value)> values, IDataKeyedList list)
        {
            var actualValues = values.ToDictionary(v => v.Name, v => v.Value);
            var keys = new string[list.KeyNames.Count];
            for (var i = 0; i < keys.Length; ++i)
            {
                var name = list.KeyNames[i];
                if (!DataSerializer.TryGetKeyType(list, name, out var type))
                    throw new NotImplementedException();
                if(!actualValues.TryGetValue(name, out var value))
                    throw new NotImplementedException();
                var strVal = value.ToYangValue();
                if (!type.ValidateAndGetCanonicalValue(strVal).Try(out var canonical, out var error))
                    throw new NotImplementedException();
                if (!type.Validate(strVal ?? string.Empty).Try(out error))
                    throw new NotImplementedException();
                keys[i] = canonical;
            }
            return new (keys);
        }
        
        public static ListKeySet Create(IDataKeyedListItem item, IReadOnlyList<QualifiedName> keyNames)
        {
            switch (keyNames.Count)
            {
                case 0:
                    throw new InvalidOperationException($"Cannot create {nameof(ListKeySet)} for a list with no keys.");
                case 1:
                    return new (GetValue(item, keyNames[0]));
                default:
                    return new (GetKeys(item, keyNames));
            }
        }

        private static string[] GetKeys(IDataKeyedListItem item, IReadOnlyList<QualifiedName> keyNames)
        {
            var keys = new string[keyNames.Count];
            for (var i = 0; i < keys.Length; ++i)
            {
                keys[i] = GetValue(item, keyNames[i]);
            }
            return keys;
        }

        private static string GetValue(IDataKeyedListItem item, QualifiedName name)
        {
            // TODO: Put this somewhere more general.  We need to be able to get values of descendent leafs.
            var child = item.Children.GetValueOrDefault(name) as IDataLeaf;
            return child?.Value ?? string.Empty;
        }

        private sealed class ListKeyValueEqualityComparer : IEqualityComparer<ListKeySet>
        {
            public bool Equals(ListKeySet? x, ListKeySet? y)
            {
                if (x is null && y is null)
                    return true;
                if (x is null || y is null)
                    return false;
                return this.CheckArray(x, y);
            }

            private bool CheckArray(ListKeySet x, ListKeySet y)
            {
                if (x.KeyCount != y.KeyCount)
                    return false;
                for (var i = 0; i < x.KeyCount; ++i)
                {
                    if (x.GetKeyValue(i) != y.GetKeyValue(i))
                        return false;
                }
                return true;
            }


            public int GetHashCode(ListKeySet obj)
            {
                var hashCode = new HashCode();
                hashCode.Add(obj.KeyCount);
                for (var i = 0; i < obj.KeyCount; ++i)
                {
                    hashCode.Add(obj.GetKeyValue(i));
                }
                return hashCode.ToHashCode();
            }
        }

        public static IEqualityComparer<ListKeySet> EqualityComparer { get; } = new ListKeyValueEqualityComparer();


    }
}