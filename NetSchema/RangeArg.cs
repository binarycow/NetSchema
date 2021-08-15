using System;

namespace NetSchema
{
    public readonly struct UnsignedRangeArg : IEquatable<UnsignedRangeArg>
    {
        private readonly bool? isMax;
        private readonly ulong value;

        private UnsignedRangeArg(bool? isMax, ulong value)
        {
            this.isMax = isMax;
            this.value = value;
        }

        public override string ToString() => this.isMax switch
        {
            true => "max",
            false => "min",
            null => this.value.ToString() ?? "0"
        };

        public static UnsignedRangeArg Value(ulong value) => new (null, value);
        public static UnsignedRangeArg Min() => new (false, default);
        public static UnsignedRangeArg Max() => new (true, default);

        public bool Equals(UnsignedRangeArg other) => this.isMax == other.isMax && this.value == other.value;
        public override bool Equals(object? obj) => obj is UnsignedRangeArg other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(this.isMax, this.value);
        public static bool operator ==(UnsignedRangeArg left, UnsignedRangeArg right) => left.Equals(right);
        public static bool operator !=(UnsignedRangeArg left, UnsignedRangeArg right) => !left.Equals(right);
    }
    
    public readonly struct SignedRangeArg : IEquatable<SignedRangeArg>
    {
        private readonly bool? isMax;
        private readonly long value;

        private SignedRangeArg(bool? isMax, long value)
        {
            this.isMax = isMax;
            this.value = value;
        }

        public static SignedRangeArg Value(long value) => new (null, value);
        public static SignedRangeArg Min() => new (false, default);
        public static SignedRangeArg Max() => new (true, default);
        
        public override string ToString() => this.isMax switch
        {
            true => "max",
            false => "min",
            null => this.value.ToString() ?? "0"
        };
        
        public bool Equals(SignedRangeArg other) => this.isMax == other.isMax && this.value == other.value;
        public override bool Equals(object? obj) => obj is SignedRangeArg other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(this.isMax, this.value);
        public static bool operator ==(SignedRangeArg left, SignedRangeArg right) => left.Equals(right);
        public static bool operator !=(SignedRangeArg left, SignedRangeArg right) => !left.Equals(right);
    }
}