
using NetSchema.Common;

namespace NetSchema.Types
{
    public interface IBuiltinSchemaType : IUsableType
    {
        public Result<string> ValidateAndGetCanonicalValue(string input, IUsableType derivedType);
    }
    // ReSharper disable once TypeParameterCanBeVariant
    public interface IBuiltinSchemaType<T> : IUsableType<T>, IBuiltinSchemaType
        where T : notnull
    {
        public Result<string> ValidateAndGetCanonicalValue(string input, IUsableType<T> derivedType);
    }
}