using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using NetSchema.Common;
using NetSchema.Data.Collections;
using NetSchema.Data.Nodes;

namespace NetSchema.Data.Dynamic
{
    internal class DynamicDictionary : DynamicDataObject
    {
        private readonly IDataKeyedList dataObject;

        public DynamicDictionary(IDataKeyedList dataObject) : base(dataObject)
        {
            this.dataObject = dataObject;
        }

        private IEnumerable<(QualifiedName Name, object? Value)> GetKeysFromIndexers(GetIndexBinder binder, IEnumerable<object?> args)
        {
            if (binder.CallInfo?.ArgumentNames is null) throw new NotImplementedException();
            var names = binder.CallInfo.ArgumentNames;
            var numberOfArguments = binder.CallInfo.ArgumentCount;
            var numberOfNames = names.Count;
            var allNames = Enumerable.Repeat<string?>(null, numberOfArguments - numberOfNames).Concat(names);
            var foo = allNames.Zip(args, (flag, value) => (Name: flag, Value: value));
            foreach (var (name, value) in foo)
            {
                if (!TryGetQName(name, out var qName))
                    throw new NotImplementedException();
                yield return (qName, value);
            }
        }

        public override bool TryGetIndex(
            GetIndexBinder binder,
            object?[] indexes,
            [NotNullWhen(true)] out object? result
        )
        {
            var keys = GetKeysFromIndexers(binder, indexes).ToList().AsReadOnly();
            if (!TryGetExistingItem(binder, indexes, out var item, keys))
            {
                item = DataSerializer.CreateListItem(this.dataObject);
                UpdateItem(item, keys);
                DataSerializer.AddChildToNode(this.dataObject, item);
            }
            result = Create(item);
            return true;
        }

        private void UpdateItem(IDataKeyedListItem item, ReadOnlyCollection<(QualifiedName Name, object? Value)> keys)
        {
            foreach (var (name, value) in keys)
            {
                if(!DataEvaluator.TrySetValue(item, name, value))
                    throw new NotImplementedException();
            }
        }

        private bool TryGetExistingItem(
            GetIndexBinder binder, 
            IEnumerable<object?> indexes, 
            [NotNullWhen(true)] out IDataKeyedListItem? item,
            IReadOnlyList<(QualifiedName Name, object? Value)> keys
        )
        {
            item = default;
            if (this.dataObject.Children.Count == 0)
                return false;
            var result = dataObject.Children.TryGetValue(ListKeySet.Create(keys, this.dataObject));
            return result.Try(out item);
        }

        public override bool TrySetIndex(
            SetIndexBinder binder, 
            object[] indexes, 
            object? value
        ) => throw new NotImplementedException();
        public override bool TryGetMember(
            GetMemberBinder binder,
            [NotNullWhen(true)] out object? result
        ) => throw new NotImplementedException();
        public override bool TrySetMember(
            SetMemberBinder binder,
            object? value
        ) => throw new NotImplementedException();
    }
}