#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Data;
using NetSchema.Data.Nodes;
using NetSchema.Resolve.Nodes;

namespace NetSchema.Xml
{
    internal static class XmlDataReader
    {
        public static Result<IDataTree> Deserialize(IResolvedSchema schema, IEnumerable<XElement> documentRoot)
        {
            var tree = schema.CreateDataTree();
            if (!ProcessElements(tree, documentRoot).Try(out var error))
                return (Result<IDataTree>)error;
            return new(tree);
        }

        private static Result ProcessElements(IDataObject parent, IEnumerable<XElement> elements)
        {
            foreach (var element in elements)
            {
                if (!ProcessItem(parent, element).Try(out var error))
                    return error;
            }
            return Result.SuccessfulResult;
        }

        private static Result ProcessItem(IDataObject parent, XElement element)
        {
            var qName = QualifiedName.FromXName(element.Name, parent);
            var kind = DataSerializer.GetKind(parent, qName);
            return kind switch
            {
                SerializationKind.Container => ProcessContainer(parent, qName, element),
                SerializationKind.Leaf => ProcessLeaf(parent, qName, element),
                SerializationKind.KeyedList => ProcessKeyedList(parent, qName, element),
                SerializationKind.UnkeyedList => ProcessUnkeyedList(parent, qName, element),
                SerializationKind.AnyData => throw new NotImplementedException(),
                SerializationKind.LeafList => ProcessLeafList(parent, qName, element),
                _ => throw new NotImplementedException(),
            };
        }
        
        private static Result ProcessContainer(IDataObject parent, QualifiedName qName, XElement element)
        {
            if(!DataSerializer.TryGetContainer(parent, qName, out var container))
                throw new NotImplementedException();

            return !PopulateContainerChildren(container, element).Try(out var error) 
                ? error 
                : DataSerializer.AddChildToNode(parent, container);
        }

        private static Result PopulateContainerChildren(IContainerLikeDataNode container, XElement element)
        {
            foreach (var item in element.Elements())
            {
                if (!ProcessItem(container, item).Try(out var error))
                {
                    return error;
                }
            }
            return Result.SuccessfulResult;
        }

        private static Result ProcessLeafList(IDataObject parent, QualifiedName qName, XElement element)
        {
            if(!DataSerializer.TryGetLeafList(parent, qName, out var list))
                throw new NotImplementedException();
            _ = DataSerializer.AddChildToNode(parent, list); //Ignore output, we end up "adding" the same list multiple times in XML representation
            return list.TryAddValue(GetText(element));
        }

        private static Result ProcessUnkeyedList(IDataObject parent, QualifiedName qName, XElement element)
        {
            if(!DataSerializer.TryGetUnkeyedList(parent, qName, out var list))
                throw new NotImplementedException();
            _ = DataSerializer.AddChildToNode(parent, list); //Ignore output, we end up "adding" the same list multiple times in XML representation
            var listItem = DataSerializer.CreateListItem(list);
            if (!PopulateContainerChildren(listItem, element).Try(out var error))
                return error;
            return DataSerializer.AddChildToNode(list, listItem);
        }

        private static Result ProcessKeyedList(IDataObject parent, QualifiedName qName, XElement element)
        {
            if(!DataSerializer.TryGetKeyedList(parent, qName, out var list))
                throw new NotImplementedException();
            _ = DataSerializer.AddChildToNode(parent, list); //Ignore output, we end up "adding" the same list multiple times in XML representation
            var listItem = DataSerializer.CreateListItem(list);
            if (!PopulateContainerChildren(listItem, element).Try(out var error))
                return error;
            return DataSerializer.AddChildToNode(list, listItem);
        }

        private static Result ProcessLeaf(IDataObject parent, QualifiedName qName, XElement element)
        {
            if(!DataSerializer.TryGetLeaf(parent, qName, out var leaf))
                throw new NotImplementedException();
            return !leaf.SetValue(GetText(element)).Try(out var error) 
                ? error 
                : DataSerializer.AddChildToNode(parent, leaf);
        }

        private static string GetText(XElement element) 
            => element.Nodes().OfType<XText>().FirstOrDefault()?.Value ?? string.Empty;
    }
}