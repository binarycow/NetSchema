using System.Collections.Generic;
using NetSchema.Common;

namespace NetSchema.Restrictions
{
    public interface ITypeRestriction
    {
        Result Validate(string value);
    }
    public interface ITypeRestriction<T>
    {
        Result Validate(T value);
    }
}