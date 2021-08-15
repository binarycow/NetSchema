#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using NetSchema.Common;
using NetSchema.Data;
using NetSchema.Data.Nodes;
using NetSchema.Types;
using Newtonsoft.Json.Linq;

namespace NetSchema.Json
{
    internal static class JsonDataWriter
    {
        public static string Serialize(IDataTree dataTree)
        {
            var moduleNames = new Stack<string>();
            var parent = new JObject();
            SerializeChildrenWithoutNameStack(parent, dataTree, moduleNames);
            return parent.ToString();
        }

        private static JObject SerializeChildren(JObject parent, IDataNode dataObject, Stack<string> moduleNames)
        {
            moduleNames.Push(dataObject.Name.ModuleName);
            SerializeChildrenWithoutNameStack(parent, dataObject, moduleNames);
            moduleNames.Pop();
            return parent;
        }
        
        private static void SerializeChildrenWithoutNameStack(JObject parent, IDataObject dataObject, Stack<string> moduleNames)
        {
            var children = DataSerializer.GetChildren(dataObject);
            foreach (var child in children)
            {
                var name = GetJsonName(child.Name, moduleNames);
                var item = SerializeItem(child, moduleNames);
                parent.Add(name, item);
            }
        }

        private static JToken SerializeChildren(JArray parent, IDataList dataObject, Stack<string> moduleNames)
        {
            moduleNames.Push(dataObject.Name.ModuleName);
            foreach (var child in DataSerializer.GetChildren(dataObject))
            {
                parent.Add(SerializeItem(child, moduleNames));
            }
            moduleNames.Pop();
            return parent;
        }
        
        private static string GetJsonName(QualifiedName qName, Stack<string> moduleNames)
        {
            return moduleNames.Count == 0 || moduleNames.Peek() != qName.ModuleName
                ? qName.ToString()
                : qName.LocalName;
        }

        private static JToken SerializeItem(IDataNode child, Stack<string> moduleNames)
        {
            return child switch
            {
                IContainerLikeDataNode container => SerializeChildren(new (), container, moduleNames),
                IDataLeaf leaf => CreateValue(leaf),
                IDataKeyedList list => SerializeChildren(new (), list, moduleNames),
                IDataUnkeyedList list => SerializeChildren(new (), list, moduleNames),
                IDataLeafList list => new JArray(CreateValues(list)),
                _ => throw new NotImplementedException()
            };
        }

        private static IEnumerable<JValue> CreateValues(IDataLeafList leafList) => leafList.Values.Select(value => CreateValue(leafList.Type, value));

        private static JValue CreateValue(IDataLeaf dataLeaf) => CreateValue(dataLeaf.Type, dataLeaf.Value);
        private static JValue CreateValue(IUsableType type, string value)
        {
            return type.Kind switch
            {
                TypeKind.String => new (value),
                TypeKind.Boolean when value == "true" => new (true),
                TypeKind.Boolean => new (false),
                _ => throw new NotImplementedException()
            };
        }

    }
}