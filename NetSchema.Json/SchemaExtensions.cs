using NetSchema.Common;
using NetSchema.Data;
using NetSchema.Data.Nodes;
using NetSchema.Json;
using NetSchema.Resolve.Nodes;

// ReSharper disable once CheckNamespace
namespace NetSchema
{
    public static class SchemaExtensions
    {
        public static Result<IDataTree> DeserializeDataTreeJson(this IResolvedSchema schema, string json) 
            => JsonDataReader.Deserialize(schema, json);

        public static string ToJson(this IDataTree dataTree) 
            => JsonDataWriter.Serialize(dataTree);
    }
}