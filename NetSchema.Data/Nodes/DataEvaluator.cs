using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using NetSchema.Common;

namespace NetSchema.Data.Nodes
{
    public static class DataEvaluator
    {
        
        public static bool TrySetValue(IDataLeafList list, IEnumerable values)
        {
            list.Clear();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var item in values)
            {
                if (!list.TryAddValue(item))
                    return false;
            }
            return true;
        }
        
        public static bool TryFindOrCreate(
            this IDataObject parent,
            QualifiedName qName,
            [NotNullWhen(true)] out IDataNode? node
        )
        {
            if (!DataSerializer.TryGetDataItem(parent, qName, out node))
                return false;
            _ = DataSerializer.AddChildToNode(parent, node); // Ignore result
            return true;
        }
        
        public static bool TrySetValue(IDataNode parent, QualifiedName qName, object? value)
        {
            if (!DataSerializer.TryGetDataItem<IDataLeaf>(parent, qName, out var node))
                throw new NotImplementedException();
            if (!node.SetValue(value).Try(out var error))
                throw new ArgumentException(error.ErrorMessage ?? $"'{value}' is not a valid value for leaf '{node.Name}'", nameof(value));
            DataSerializer.AddChildToNode(parent, node);
            return true;
        }

        public static bool TryFindOrCreate(
            IDataUnkeyedList parent,
            int index, 
            [NotNullWhen(true)] out IDataUnkeyedListItem? node
        )
        {
            node = default;
            if (index < 0)
                return false;
            if (index == parent.Children.Count)
            {
                node = new DataUnkeyedListItem(parent);
                DataSerializer.AddChildToNode(parent, node);
                return true;
            }
            node = parent.Children[index];
            return node is not null;
        }
        public static bool TryGetChild(
            this IDataObject dataObject, 
            QualifiedName name, 
            [NotNullWhen(true)] out IDataNode? result
        )
        {
            return dataObject switch
            {
                IDataTree tree => tree.Children.TryGetValue(name, out result),
                IContainerLikeDataNode container => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };
        }
        
        public static bool TryGetValue(
            this IDataObject dataObject, 
            QualifiedName name, 
            [NotNullWhen(true)] out string? result
        )
        {
            result = default;
            if (!dataObject.TryGetChild(name, out var child) || child is not IDataLeaf leaf)
                return false;
            result = leaf.Value;
            return true;
        }
        
        public static bool TryGetValueObject(
            this IDataObject dataObject, 
            QualifiedName name, 
            [NotNullWhen(true)] out object? result
        )
        {
            result = default;
            if (!dataObject.TryGetChild(name, out var child) || child is not IDataLeaf leaf)
                return false;
            result = leaf.Type.GetCSharpValue(leaf.Value);
            return result is not null;
        }

    }
}