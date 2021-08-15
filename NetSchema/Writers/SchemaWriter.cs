using System.Collections.Generic;
using System.Linq;
using NetSchema.Syntax;

namespace NetSchema.Writers
{
    internal static class SchemaWriter
    {
        public static IEnumerable<ISchemaDocument> Write(IWritable schema, ISchemaWriter writer)
        {
            foreach (var module in schema.Modules)
            {
                writer.OpenDocument(module.Name, module.RevisionDate);
                WriteSyntaxNode(writer, module.CreateSyntaxNode());
                yield return writer.CloseDocument();
            }
        }

        private static void WriteSyntaxNode(ISchemaWriter writer, ISyntaxNode node)
        {
            var opened = false;
            foreach (var child in node.GetChildren().OrderBy(x => x, CanonicalSorter.Instance))
            {
                if (opened == false)
                {
                    writer.OpenStatement(node.Type, node.Argument);
                    opened = true;
                }
                WriteSyntaxNode(writer, child);
            }
            if (opened is true)
                writer.CloseStatement(node.Type, node.Argument);
            else
                writer.WriteStatement(node.Type, node.Argument);
        }
    }
}