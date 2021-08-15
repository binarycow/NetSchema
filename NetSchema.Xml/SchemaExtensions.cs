using System;
using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Common.Exceptions;
using NetSchema.Data;
using NetSchema.Data.Nodes;
using NetSchema.Resolve.Nodes;
using NetSchema.Xml;

// ReSharper disable once CheckNamespace
namespace NetSchema
{
    public static class SchemaExtensions
    {
        public static Result<IDataTree> DeserializeDataTreeXml(this IResolvedSchema schema, XDocument document, XName xmlWrapperName)
        {
            if (document.Root is null)
            {
                throw new NetSchemaSerializationException("Root element is null");
            }
            if (document.Root.Name != xmlWrapperName)
            {
                throw new NetSchemaSerializationException($"Unexpected element name {document.Root.Name}");
            }
            return XmlDataReader.Deserialize(schema, document.Root.Elements());
        }

        public static XElement ToXml(this IDataTree dataTree, XName xmlWrapperName) 
            => new XElement(xmlWrapperName, XmlDataWriter.Serialize(dataTree));
    }
}