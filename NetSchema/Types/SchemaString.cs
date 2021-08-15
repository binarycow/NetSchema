#nullable enable

using System;
using System.Collections.Generic;
using NetSchema.Common;
using NetSchema.Restrictions;

namespace NetSchema.Types
{
    internal class SchemaString : BuiltinType
    {
        public SchemaString() : base("string")
        {
        }
        public override Result Validate(string value) => Result.SuccessfulResult;
        public override TypeKind Kind => TypeKind.String;
        public override object? GetCSharpValue(string value) => value;
        protected override Result<string> GetCanonicalValue(string input) => input;
    }
}