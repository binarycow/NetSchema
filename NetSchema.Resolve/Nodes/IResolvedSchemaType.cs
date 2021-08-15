using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Restrictions;
using NetSchema.Types;

#nullable enable

namespace NetSchema.Resolve.Nodes
{
    public interface IResolvedSchemaType : IResolvedNamedSchemaNode
    {
        IReadOnlyList<ITypeRestriction> Restrictions { get; }

        IUsableType GetUsableType(QualifiedName? name);
    }

    internal class ResolvedSchemaType : IResolvedSchemaType
    {
        private readonly IUsableType wrapped;
        public ResolvedSchemaType(IUsableType wrapped)
        {
            this.wrapped = wrapped;
        }

        public IReadOnlyList<ITypeRestriction> Restrictions { get; init; } = Array.Empty<ITypeRestriction>();
        public IUsableType GetUsableType(QualifiedName? name)
        {
            if (Restrictions.Count == 0 && name is null)
            {
                return this.wrapped;
            }

            return new DerivedType(name ?? QualifiedName, this.wrapped)
            {
                Restrictions = Restrictions,
            };
        }
        public string Name => this.wrapped.QualifiedName.LocalName;
        public QualifiedName QualifiedName => this.wrapped.QualifiedName;
        StatementType IResolvedSchemaNode.StatementType => StatementType.Type;
        string IResolvedSchemaNode.Argument => Name;
    }
}