using System;
using System.Collections.Generic;
using NetSchema.Common;
using NetSchema.Restrictions;

namespace NetSchema.Types
{
    public interface IUsableType
    {
        QualifiedName QualifiedName { get; }
        IUsableType? BaseType { get; }
        IBuiltinSchemaType RootType { get; }
        TypeKind Kind { get; }
        object? GetCSharpValue(string value);
        IReadOnlyList<ITypeRestriction> Restrictions { get; }
    }
    
    public interface IUsableType<T> : IUsableType 
        where T : notnull
    {
        new IUsableType<T>? BaseType { get; }
        new IBuiltinSchemaType<T> RootType { get; }
    }

    public class DerivedType : IUsableType
    {
        public DerivedType(QualifiedName name, IUsableType baseType)
        {
            this.QualifiedName = name;
            this.BaseType = baseType;
        }

        public IReadOnlyList<ITypeRestriction> Restrictions { get; init; } = Array.Empty<ITypeRestriction>();

        public QualifiedName QualifiedName { get; }
        public IUsableType BaseType { get; }
        public IBuiltinSchemaType RootType => BaseType.RootType;


        public TypeKind Kind => RootType.Kind;
        public object? GetCSharpValue(string value) => RootType.GetCSharpValue(value);
    }
}