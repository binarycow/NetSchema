using NetSchema.Resolve;
using NetSchema.Resolve.Nodes;
using NetSchema.Syntax;

#nullable enable

// ReSharper disable once CheckNamespace
namespace NetSchema
{
    public static class SchemaResolveExtensions
    {
        public static IResolvedSchema Resolve(this ISyntaxSchema schema)
        {
            var table = TableBuilder.BuildTables(schema);
            return Resolver.Resolve(table);
        }
    }
}