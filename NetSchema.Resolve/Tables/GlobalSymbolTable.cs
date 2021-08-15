using System;
using System.Diagnostics.CodeAnalysis;
using NetSchema.Syntax;
using NetSchema.Types;

namespace NetSchema.Resolve.Tables
{
    internal class GlobalSymbolTable : SymbolTable
    {
        public override ISyntaxNode? OriginatingNode => null;
        public override SymbolTable? Parent => null;
        public override GlobalSymbolTable Globals => this;

        public static bool TryGetBuiltinType(string name, [NotNullWhen(true)] out IUsableType? resolved)
        {
            resolved = name switch
            {
                "string" => BuiltinTypes.String,
                "boolean" => BuiltinTypes.Boolean,
                "binary" => throw new NotImplementedException(),
                "bits" => throw new NotImplementedException(),
                "decimal64" => throw new NotImplementedException(),
                "empty" => throw new NotImplementedException(),
                "enumeration" => throw new NotImplementedException(),
                "identityref" => throw new NotImplementedException(),
                "instance-identifier" => throw new NotImplementedException(),
                "int8" => throw new NotImplementedException(),
                "int16" => throw new NotImplementedException(),
                "int32" => throw new NotImplementedException(),
                "int64" => throw new NotImplementedException(),
                "leafref" => throw new NotImplementedException(),
                "uint8" => throw new NotImplementedException(),
                "uint16" => throw new NotImplementedException(),
                "uint32" => throw new NotImplementedException(),
                "uint64" => throw new NotImplementedException(),
                "union" => throw new NotImplementedException(),
                _ => null,
            };
            return resolved is not null;
        }
    }
}