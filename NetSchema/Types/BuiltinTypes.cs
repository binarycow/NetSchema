using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NetSchema.Common;
using NetSchema.Restrictions;

#nullable enable

namespace NetSchema.Types
{
    public static class BuiltinTypes
    {
        public static readonly IBuiltinSchemaType Boolean = new SchemaBoolean();
        public static readonly IBuiltinSchemaType String = new SchemaString();
    }

    internal abstract class BuiltinType : IBuiltinSchemaType
    {
        protected BuiltinType(string name) => this.QualifiedName = new (string.Empty, name);
        public string Name => QualifiedName.LocalName;
        public QualifiedName QualifiedName { get; }
        IUsableType? IUsableType.BaseType => null;
        IBuiltinSchemaType IUsableType.RootType => this;
        public abstract Result Validate(string value);
        public abstract TypeKind Kind { get; }
        public abstract object? GetCSharpValue(string value);
        public IReadOnlyList<ITypeRestriction> Restrictions { get; } = Array.Empty<ITypeRestriction>();

        protected abstract Result<string> GetCanonicalValue(string input);
        
        
        public Result<string> ValidateAndGetCanonicalValue(string input, IUsableType derivedType)
        {
            if (!GetCanonicalValue(input).Try(out var canonical, out var error))
                return (Result<string>)error;
            var current = derivedType;
            while (current is not null)
            {
                if (!current.Restrictions.Validate(canonical, Kind).Try(out error))
                    return (Result<string>) error;
                current = current.BaseType;
            }
            return canonical;
        }
    }

    internal abstract class BuiltinType<T> : IBuiltinSchemaType<T>
        where T : struct
    {
        public abstract object? GetCSharpValue(string value);
        public abstract TypeKind Kind { get; }

        protected abstract bool TryParse(string text, out T value);
        protected abstract Result<string> GetCanonicalValue(T input);

        Result<string> IBuiltinSchemaType.ValidateAndGetCanonicalValue(string input, IUsableType derivedType)
        {
            if (derivedType is IUsableType<T> derived) 
                return this.ValidateAndGetCanonicalValue(input, derived);
            
            // TODO: Is this /really/ the right way to handle this?
            return Result<string>.CreateError($"Provided type {derivedType.QualifiedName.ToString()} has the wrong generic parameter (expected: {typeof(T)})");
        }

        public Result<string> ValidateAndGetCanonicalValue(string input, IUsableType<T> derivedType)
        {
            if (!TryParse(input, out var value))
                return Result<string>.CreateError($"'{input}' is not a valid value for {Kind}");
            if (!GetCanonicalValue(value).Try(out var canonical, out var error))
                return (Result<string>)error;
            var current = derivedType;
            while (current is not null)
            {
                if (!current.Restrictions.Validate(canonical, Kind).Try(out error))
                    return (Result<string>) error;
                current = current.BaseType;
            }
            return canonical;
        }


        protected BuiltinType(string name) => this.QualifiedName = new (string.Empty, name);
        public string Name => QualifiedName.LocalName;
        public QualifiedName QualifiedName { get; }
        public IReadOnlyList<ITypeRestriction> Restrictions { get; } = Array.Empty<ITypeRestriction>();

        IUsableType<T>? IUsableType<T>.BaseType => null;
        IUsableType? IUsableType.BaseType => null;
        IBuiltinSchemaType<T> IUsableType<T>.RootType => this;
        IBuiltinSchemaType IUsableType.RootType => this;
    }
}