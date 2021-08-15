using System;
using NetSchema.Common;

#nullable enable

namespace NetSchema.Types
{
    internal class SchemaBoolean : BuiltinType<bool>
    {
        public SchemaBoolean() : base("boolean")
        {
        }

        public override TypeKind Kind => TypeKind.Boolean;
        protected override bool TryParse(string text, out bool value) => bool.TryParse(text, out value);

        protected override Result<string> GetCanonicalValue(bool input) => input ? "true" : "false";

        // ReSharper disable once HeapView.BoxingAllocation
        public override object? GetCSharpValue(string value) => value switch
        {
            "true" => true,
            "false" => false,
            _ => throw new ArgumentException(nameof(value)),
        };
    }
}