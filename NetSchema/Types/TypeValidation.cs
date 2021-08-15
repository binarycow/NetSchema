using System;
using System.Collections.Generic;
using System.Linq;
using NetSchema.Common;
using NetSchema.Restrictions;

namespace NetSchema.Types
{
    public static class TypeValidation
    {
        public static Result<string> ValidateAndGetCanonicalValue(this IUsableType type, string input)
        {
            if (!type.RootType.ValidateAndGetCanonicalValue(input, type).Try(out var canonical, out var error))
                return (Result<string>)error;
            return canonical;
        }

        public static Result Validate(this IUsableType type, string input) =>
            type.ValidateAndGetCanonicalValue(input);

        public static Result Validate(this IEnumerable<ITypeRestriction> restrictions, string input, TypeKind typeKind)
        {   
            var predicate = GetPredicate(typeKind);
            restrictions = restrictions.Where(predicate);
            return restrictions.Select(r => r.Validate(input)).All();
        }

        private static Func<ITypeRestriction, bool> GetPredicate(TypeKind typeKind) => typeKind switch
        {
            TypeKind.Boolean => None,
            TypeKind.Empty => None,
            TypeKind.String => GetStringRestrictions,
            TypeKind.Binary => throw new NotImplementedException(),
            TypeKind.Bits => throw new NotImplementedException(),
            TypeKind.Decimal64 => throw new NotImplementedException(),
            TypeKind.Enumeration => throw new NotImplementedException(),
            TypeKind.IdentityRef => throw new NotImplementedException(),
            TypeKind.InstanceIdentifier => throw new NotImplementedException(),
            TypeKind.Int8 => throw new NotImplementedException(),
            TypeKind.Int16 => throw new NotImplementedException(),
            TypeKind.Int32 => throw new NotImplementedException(),
            TypeKind.Int64 => throw new NotImplementedException(),
            TypeKind.LeafRef => throw new NotImplementedException(),
            TypeKind.UInt8 => throw new NotImplementedException(),
            TypeKind.UInt16 => throw new NotImplementedException(),
            TypeKind.UInt32 => throw new NotImplementedException(),
            TypeKind.UInt64 => throw new NotImplementedException(),
            TypeKind.Union => throw new NotImplementedException(),
            TypeKind.Unknown => throw new ArgumentOutOfRangeException(nameof(typeKind)),
            _ => throw new ArgumentOutOfRangeException(nameof(typeKind)),
        };

        private static bool None(ITypeRestriction restriction) => false;
        private static bool GetStringRestrictions(ITypeRestriction restrictions) => restrictions switch
        {
            IReadOnlySchemaPattern => true,
            _ => throw new NotImplementedException()
        };
    }
}