using System;
using System.Collections.Generic;
using System.Linq;
using NetSchema.Common;

namespace NetSchema.Restrictions
{

    public interface IReadOnlySchemaLength : ITypeRestriction
    {
        IReadOnlyList<(UnsignedRangeArg Low, UnsignedRangeArg? High)> Lengths { get; }
    }
}