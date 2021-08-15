using System;

namespace NetSchema.Common
{
    public enum TypeKind
    {
        Unknown = 0,
        Binary,
        Bits,
        Boolean,
        Decimal64,
        Empty,
        Enumeration,
        IdentityRef,
        InstanceIdentifier,
        Int8,
        Int16,
        Int32,
        Int64,
        LeafRef,
        String,
        UInt8,
        UInt16,
        UInt32,
        UInt64,
        Union,
    }
}