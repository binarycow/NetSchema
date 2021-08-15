#nullable enable

using System;
using NetSchema.Common;
using NetSchema.Data;
using NetSchema.Data.Nodes;
using NetSchema.Resolve.Nodes;
using Newtonsoft.Json.Linq;

namespace NetSchema.Json
{
    internal static class JsonDataReader
    {
        public static Result<IDataTree> Deserialize(IResolvedSchema schema, string json)
        {
            var doc = JObject.Parse(json);
            var tree = schema.CreateDataTree();
            return ProcessTopLevel(tree, doc);
        }

        private static Result<IDataTree> ProcessTopLevel(IDataTree tree, JObject element)
        {
            foreach (var item in element)
            {
                if (item.Key is null || item.Value is null)
                {
                    continue;
                }
                if (!ValidateJsonName(item.Key, out var qName, requireModuleName: true))
                {
                    return Result<IDataTree>.CreateError($"A namespace-qualified member name MUST be used for all members of a top-level JSON object (Provided: '{item.Key}') (RFC 7951, Section 4)");
                }
                if (!ProcessItem(tree, qName, item.Value).Try(out var error))
                {
                    return (Result<IDataTree>)error;
                }
            }
            return Result<IDataTree>.CreateSuccess(tree);
        }

        private static bool ValidateJsonName(string name, out QualifiedName qName, bool requireModuleName = false)
        {
            if (!QualifiedName.TryParse(name, out qName))
            {
                return false;
            }
            return requireModuleName == false || qName.ModuleName.IsNotNullOrWhiteSpace();
        }

        private static Result ProcessListItem(IDataList list, JObject item)
        {
            var listItem = DataSerializer.CreateListItem(list);
            if (!PopulateContainerChildren(listItem, item).Try(out var error))
                return error;
            return DataSerializer.AddChildToNode(list, listItem);
        }
        
        private static Result ProcessItem(IDataObject parent, QualifiedName qName, JToken element)
        {
            var kind = DataSerializer.GetKind(parent, qName);
            return (kind, element) switch
            {
                (SerializationKind.Container, JObject obj) => ProcessContainer(parent, qName, obj),
                (SerializationKind.Container, _) => throw new NotImplementedException(),
                (SerializationKind.Leaf, JValue value) => ProcessLeaf(parent, qName, value),
                (SerializationKind.Leaf, _) => throw new NotImplementedException(),
                (SerializationKind.KeyedList, JArray array) => ProcessKeyedList(parent, qName, array),
                (SerializationKind.KeyedList, _) => throw new NotImplementedException(),
                (SerializationKind.UnkeyedList, JArray array) => ProcessUnkeyedList(parent, qName, array),
                (SerializationKind.UnkeyedList, _) => throw new NotImplementedException(),
                (SerializationKind.AnyData, _) => throw new NotImplementedException(),
                (SerializationKind.AnyXml, _) => throw new NotImplementedException(),
                (SerializationKind.LeafList, JArray array) => ProcessLeafList(parent, qName, array),
                (SerializationKind.LeafList, _) => throw new NotImplementedException(),
                _ => throw new NotImplementedException()
            };
        }

        private static Result ProcessUnkeyedList(IDataObject parent, QualifiedName qName, JArray array)
        {
            if(!DataSerializer.TryGetUnkeyedList(parent, qName, out var list))
                throw new NotImplementedException();
            
            return !PopulateListChildren(list, array).Try(out var error) 
                ? error 
                : DataSerializer.AddChildToNode(parent, list);
        }



        private static Result ProcessKeyedList(IDataObject parent, QualifiedName qName, JArray array)
        {
            if(!DataSerializer.TryGetKeyedList(parent, qName, out var list))
                throw new NotImplementedException();
            
            return !PopulateListChildren(list, array).Try(out var error) 
                ? error 
                : DataSerializer.AddChildToNode(parent, list);
        }


        private static Result<string> GetValue(TypeKind kind, JValue value)
        {
            var valueToWrite = (type: kind, value.Value) switch
            {
                (TypeKind.String, string strVal) => strVal,
                (TypeKind.String, _) => null,
                
                (TypeKind.Boolean, bool boolVal) => boolVal ? "true" : "false",
                (TypeKind.Boolean, _) => null,
                
                _ => throw new NotImplementedException()
            };
            // ReSharper disable once MergeConditionalExpression
            return valueToWrite is null
                ? Result<string>.CreateError($"'{value.Value}' is not a valid JSON value for type kind {kind}")
                : valueToWrite;
        }

        private static Result ProcessLeafList(IDataObject parent, QualifiedName qName, JArray array)
        {
            if(!DataSerializer.TryGetLeafList(parent, qName, out var list))
                throw new NotImplementedException();
            return !PopulateLeafListChildren(list, array).Try(out var error) 
                ? error 
                : DataSerializer.AddChildToNode(parent, list);
        }
        
        private static Result ProcessLeaf(IDataObject parent, QualifiedName qName, JValue value)
        {
            if(!DataSerializer.TryGetLeaf(parent, qName, out var leaf))
                throw new NotImplementedException();
            if (!JsonDataReader.GetValue(leaf.Type.Kind, value).Try(out var valueToWrite, out var error))
            {
                return error;
            }
            return !leaf.SetValue(valueToWrite).Try(out error) ? error : DataSerializer.AddChildToNode(parent, leaf);
        }

        private static Result ProcessContainer(IDataObject parent, QualifiedName qName, JObject element)
        {
            if(!DataSerializer.TryGetContainer(parent, qName, out var container))
                throw new NotImplementedException();

            return !PopulateContainerChildren(container, element).Try(out var error) 
                ? error 
                : DataSerializer.AddChildToNode(parent, container);
        }
        
        
        // ReSharper disable once SuggestBaseTypeForParameter
        private static Result PopulateContainerChildren(IContainerLikeDataNode container, JObject element)
        {
            foreach (var item in element)
            {
                if (item.Key is null || item.Value is null)
                {
                    continue;
                }
                if (!JsonDataReader.ValidateJsonName(item.Key, out var qName))
                {
                    return Result.CreateError($"JSON property is not a valid identifier (Provided: '{item.Key}') (RFC 7951, Section 4)");
                }
                if (!ProcessItem(container, DataSerializer.HandleDefaultModuleNames(qName, container), item.Value).Try(out var error))
                {
                    return error;
                }
            }
            return Result.SuccessfulResult;
        }
        
        private static Result PopulateLeafListChildren(IDataLeafList list, JArray array)
        {
            foreach (var item in array)
            {
                if (item is not JValue value)
                {
                    return Result.CreateError($"Expected a JSON Value for a leaf-list child; received {item.Type}");
                }
                if (!GetValue(list.Type.Kind, value).Try(out var valueToWrite, out var error))
                {
                    return error;
                }

                if (!list.TryAddValue(valueToWrite).Try(out error))
                {
                    return error;
                }
            }
            return Result.SuccessfulResult;
        }
        
        private static Result PopulateListChildren(IDataUnkeyedList list, JArray array)
        {
            foreach (var item in array)
            {
                if (item is not JObject itemObject)
                {
                    return Result.CreateError($"Expected a JSON Object for a list child; received {item.Type}");
                }
                if (!ProcessListItem(list, itemObject).Try(out var error))
                {
                    return error;
                }
            }
            return Result.SuccessfulResult;
        }
        private static Result PopulateListChildren(IDataKeyedList list, JArray array)
        {
            foreach (var item in array)
            {
                if (item is not JObject itemObject)
                {
                    return Result.CreateError($"Expected a JSON Object for a list child; received {item.Type}");
                }
                if (!ProcessListItem(list, itemObject).Try(out var error))
                {
                    return error;
                }
            }
            return Result.SuccessfulResult;
        }


    }
}