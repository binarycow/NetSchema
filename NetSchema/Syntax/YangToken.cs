using System;
using System.Diagnostics;

namespace NetSchema.Syntax
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal readonly struct YangToken : IEquatable<YangToken>
    {
        public YangTokenType Type { get; }
        public int Length { get; }

        public YangToken(YangTokenType type, int length)
        {
            this.Type = type;
            this.Length = length;
        }

        private string DebuggerDisplay => $"{Type} ({Length.ToString()})";

        public bool Equals(YangToken other) => this.Type == other.Type && this.Length == other.Length;
        public override bool Equals(object? obj) => obj is YangToken other && Equals(other);
        public override int GetHashCode() => HashCode.Combine((int)this.Type, this.Length);
        public static bool operator ==(YangToken left, YangToken right) => left.Equals(right);
        public static bool operator !=(YangToken left, YangToken right) => !left.Equals(right);
    }
}