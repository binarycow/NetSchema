using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using NetSchema.Data.Nodes;

namespace NetSchema.Data.Dynamic
{
    internal class DynamicList : DynamicDataObject
    {
        private readonly IDataUnkeyedList list;

        public DynamicList(IDataUnkeyedList list) : base(list) => this.list = list;

        private static bool GetIndex(IReadOnlyList<object> indexes, out int index)
        {
            index = default;
            if (indexes.Count != 1 || indexes[0] is not int intValue)
                return false;
            index = intValue;
            return true;
        }
        
        
        public override bool TryGetIndex(
            GetIndexBinder binder, 
            object[] indexes,
            [NotNullWhen(true)] out object? result
        )
        {
            result = default;
            if (!GetIndex(indexes, out var idx))
                return false;
            if (!DataEvaluator.TryFindOrCreate(this.list, idx, out var node))
                return false;
            result = Create(node);
            return true;
        }

        public override bool TrySetIndex(
            SetIndexBinder binder, 
            object[] indexes, 
            object? value
        )
        {
            throw new NotImplementedException();
        }

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