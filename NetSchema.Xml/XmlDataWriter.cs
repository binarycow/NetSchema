using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NetSchema.Data;
using NetSchema.Data.Nodes;

namespace NetSchema.Xml
{
    internal static class XmlDataWriter
    {
        public static IEnumerable<XElement> Serialize(IDataTree dataTree) => SerializeChildren(dataTree);

        private static IEnumerable<XElement> Serialize(IDataNode dataNode)
        {
            var name = dataNode.Name.ToXName(dataNode);
            return dataNode switch
            {
                IContainerLikeDataNode container => SerializeContainer(name, container).Yield(),
                IDataLeaf leaf => SerializeLeaf(name, leaf).Yield(),
                IDataList list => SerializeList(list),
                IDataLeafList list => SerializeLeafList(name, list),
                _ => throw new System.NotImplementedException(),
            };
        }

        private static IEnumerable<XElement> SerializeLeafList(XName name, IDataLeafList list) 
            => list.Values.Select(value => new XElement(name, value));

        private static IEnumerable<XElement> SerializeList(IDataList list) 
            => SerializeChildren(list);

        private static XElement SerializeLeaf(XName name, IDataLeaf leaf)
            => new(name, leaf.Value);

        private static XElement SerializeContainer(XName name, IContainerLikeDataNode container) 
            => new (name, SerializeChildren(container));

        private static IEnumerable<XElement> SerializeChildren(IDataObject dataObject)
            => DataSerializer.GetChildren(dataObject).SelectMany(Serialize);
    }
}