using System.Dynamic;
using NetSchema.Data.Dynamic;
using NetSchema.Data.Nodes;
using NetSchema.Resolve.Nodes;

namespace NetSchema.Data
{
    public static class DataSchemaExtensions
    {
        public static IDataTree CreateDataTree(this IResolvedSchema schema) => DataTree.Create(schema);

        public static DynamicObject ToDynamic(this IDataNode dataObject) => DynamicDataObject.Create(dataObject);
    }
}