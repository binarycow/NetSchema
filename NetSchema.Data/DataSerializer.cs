#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Common.Exceptions;
using NetSchema.Data.Nodes;
using NetSchema.Resolve.Nodes;
using NetSchema.Types;
using IDataObject = NetSchema.Data.Nodes.IDataObject;

namespace NetSchema.Data
{
    internal static class DataSerializer
    {
        internal static QualifiedName HandleDefaultModuleNames(QualifiedName qName, IDataNode parent) 
            => qName.ModuleName.IsNullOrWhiteSpace() ? new (parent.Name.ModuleName, qName.LocalName) : qName;
        
        internal static SerializationKind GetKind(IDataObject dataTree, QualifiedName qName)
        {
            var schemaObject = TryFindSchemaObject(dataTree, qName, out var n) ? n : null;
            return schemaObject switch
            {
                IResolvedSchemaLeaf => SerializationKind.Leaf,
                IResolvedSchemaContainer => SerializationKind.Container,
                IResolvedSchemaList { HasKeys: true } => SerializationKind.KeyedList,
                IResolvedSchemaList => SerializationKind.UnkeyedList,
                IResolvedSchemaLeafList => SerializationKind.LeafList,
                _ => throw new UnknownDerivedTypeException<IResolvedSchemaDataNode>(schemaObject),
            };
        }
        
        internal static bool TryGetKeyType(IDataKeyedList list, QualifiedName name, [NotNullWhen(true)] out IUsableType? type)
        {
            type = default;
            if (!TryFindSchemaObject(list, name, out var found) || found is not IResolvedSchemaLeaf leaf)
                return false;
            type = leaf.Type;
            return true;
        }
        
        internal static bool TryFindSchemaObject(
            IDataObject dataObject,
            QualifiedName qName, 
            [NotNullWhen(true)] out IResolvedSchemaDataNode? schemaNode
        )
        {
            return dataObject switch
            {
                IDataTree tree => TryFindSchemaObject(tree, qName, out schemaNode),
                IDataNode node => TryFindSchemaObject(node, qName, out schemaNode),
                _ => throw new UnknownDerivedTypeException<IDataObject>(dataObject)
            };
        }
        
        internal static bool TryFindSchemaObject(
            IDataNode node,
            QualifiedName qName, 
            [NotNullWhen(true)] out IResolvedSchemaDataNode? schemaNode
        )
        {
            if (node.Name.ModuleName != qName.ModuleName)
                throw new NotSupportedNetSchemaException(NotSupportedFeature.MultipleModules);
            return node.SchemaNode.DataNodes.TryGetValue(qName.LocalName, out schemaNode);
        }
        
        internal static bool TryFindSchemaObject(
            IDataTree tree,
            QualifiedName qName, 
            [NotNullWhen(true)] out IResolvedSchemaDataNode? schemaNode
        )
        {
            schemaNode = default;
            return tree.Schema.Modules.TryGetValue(qName.ModuleName, out var module) 
                   && module.DataNodes.TryGetValue(qName.LocalName, out schemaNode);
        }

        internal static bool TryGetUnkeyedList(IDataObject parent, QualifiedName qName, [NotNullWhen(true)] out IDataUnkeyedList? node)
            => DataSerializer.TryGetDataItem(parent, qName, out node);
        internal static bool TryGetLeafList(IDataObject parent, QualifiedName qName, [NotNullWhen(true)] out IDataLeafList? node)
            => DataSerializer.TryGetDataItem(parent, qName, out node);
        internal static bool TryGetKeyedList(IDataObject parent, QualifiedName qName, [NotNullWhen(true)] out IDataKeyedList? node)
            => DataSerializer.TryGetDataItem(parent, qName, out node);
        internal static bool TryGetContainer(IDataObject parent, QualifiedName qName, [NotNullWhen(true)] out IDataContainer? node)
            => DataSerializer.TryGetDataItem(parent, qName, out node);
        internal static bool TryGetLeaf(IDataObject parent, QualifiedName qName, [NotNullWhen(true)] out IDataLeaf? node)
            => DataSerializer.TryGetDataItem(parent, qName, out node);
        
        internal static bool TryGetDataItem(IDataObject parent, QualifiedName qName, [NotNullWhen(true)] out IDataNode? node)
            => DataSerializer.TryGetDataItem<IDataNode>(parent, qName, out node);

        internal static bool TryGetDataItem<T>(IDataObject parent, QualifiedName qName, [NotNullWhen(true)] out T? node)
            where T : IDataNode
        {
            node = default;
            if (TryGetExistingDataNode(parent, qName, out node))
            {
                return true;
            }
            if (!TryFindSchemaObject(parent, qName, out var schemaObject))
            {
                throw new NotImplementedException();
            }
            if (!DataSerializer.CreateNode(parent, schemaObject).Try(out var child, out var error))
            {
                return error;
            }

            if (child is not T typed)
            {
                return false;
            }
            node = typed;
            return true;
        }

        internal static Result AddChildToNode(IDataObject parent, IDataNode child) => parent.TryAddChildExtension(child);
        internal static IDataKeyedListItem CreateListItem(IDataKeyedList list) => new DataKeyedListItem(list);
        internal static IDataUnkeyedListItem CreateListItem(IDataUnkeyedList list) => new DataUnkeyedListItem(list);
        internal static IDataListItem CreateListItem(IDataList list) => list switch
        {
            IDataKeyedList typed => CreateListItem(typed),
            IDataUnkeyedList typed => CreateListItem(typed),
            _ => throw new ArgumentOutOfRangeException(nameof(list))
        };

        internal static Result<IDataNode> CreateNode(
            IDataObject parent, 
            IResolvedSchemaDataNode schemaNode
        )
        {
            return (schemaNode, parent) switch
            {
                // DataKeyedListItem
                (IResolvedSchemaList typedNode, IDataKeyedList typedParent) 
                    => new DataKeyedListItem(typedParent),
                (_, IDataKeyedList) 
                    => throw new NotImplementedException(),
                
                // DataContainer
                (IResolvedSchemaContainer typedNode, _)
                    => new DataContainer(parent, typedNode),
                
                // DataLeaf
                (IResolvedSchemaLeaf typedNode, _) 
                    => new DataLeaf(parent, typedNode),
                
                // DataLeafList
                (IResolvedSchemaLeafList typedNode, _)
                    => new DataLeafList(parent, typedNode),
                
                // DataKeyedList
                (IResolvedSchemaList { HasKeys: true } typedNode, _) 
                    => new DataKeyedList(parent, typedNode),
                
                // DataList (non-keyed)
                (IResolvedSchemaList typedNode, _) 
                    => new DataUnkeyedList(parent, typedNode),
                
                // Unknown
                _ => throw new NotImplementedException(),
            };
        }
        
        internal static bool TryGetExistingDataNode<T>(
            IDataObject parent,
            QualifiedName qName,
            [NotNullWhen(true)] out T? dataNode
        ) where T : IDataNode
        {
            dataNode = default;
            if (!TryGetExistingDataNode(parent, qName, out var item) || item is not T typed)
            {
                return false;
            }
            dataNode = typed;
            return true;
        }
        internal static bool TryGetExistingDataNode(
            IDataObject parent, 
            QualifiedName qName,
            [NotNullWhen(true)] out IDataNode? dataNode
        )
        {
            return parent switch
            {
                IContainerLikeDataObject container => container.Children.TryGetValue(qName, out dataNode),
                _ => throw new NotImplementedException()
            };
        }

        internal static IEnumerable<IDataNode> GetChildren(IDataObject dataObject)
        {
            return dataObject switch
            {
                IContainerLikeDataObject container => container.Children,
                IDataKeyedList list => list.Children,
                IDataUnkeyedList list => list.Children,
                _ => throw new NotImplementedException()
            };
        }

    }
}