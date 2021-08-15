using System.Collections.Generic;
using System.IO;
using NetSchema.Writers;

// ReSharper disable once CheckNamespace
namespace NetSchema
{
    public static class SchemaWriterExtensions
    {
        public static IEnumerable<ISchemaDocument> WriteYang(this IWritable schema, DirectoryInfo directory)
        {
            using var writer = new YangWriter(directory);
            return schema.WriteSchema(writer);
        }

        public static IEnumerable<ISchemaDocument> WriteYin(this IWritable schema, DirectoryInfo directory)
        {
            using var writer = new YinWriter(directory);
            return schema.WriteSchema(writer);
        }
    }
}