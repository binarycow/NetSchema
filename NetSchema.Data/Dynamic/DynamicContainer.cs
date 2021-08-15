using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using NetSchema.Common;
using NetSchema.Common.Exceptions;
using NetSchema.Data.Nodes;

namespace NetSchema.Data.Dynamic
{
    internal class DynamicContainer : DynamicDataObject
    {
        private readonly IContainerLikeDataNode container;
        public DynamicContainer(IContainerLikeDataNode container) : base(container) => this.container = container;

        public override bool TryGetIndex(
            GetIndexBinder binder, 
            object[] indexes,
            [NotNullWhen(true)] out object? result
        )
        {
            result = default;
            return this.TryGetQName(indexes, out var name) && this.TryGetProperty(name, out result) && result is not null;
        }

        public override bool TrySetIndex(
            SetIndexBinder binder,
            object[] indexes,
            object? value
        ) => TryGetQName(indexes, out var name) && this.TrySetProperty(name, value);

        private bool TrySetProperty(
            string? name, 
            object? value
        ) => TryGetQName(name, out var qName) && DataEvaluator.TrySetValue(this.container, qName, value);
        
        private bool TrySetProperty(
            QualifiedName name, 
            object? value
        )
        {
            if (!this.container.TryFindOrCreate(name, out var node))
                throw new NotImplementedException();
            var result = node switch
            {
                IDataLeaf leaf => leaf.SetValue(value),
                IDataLeafList list when value is IEnumerable enumerable => SetLeafList(list, enumerable),
                _ => throw new NotImplementedException(),
            };
            if (result.IsSuccess)
                return true;
            if (result.ErrorMessage is null)
                result = Result.CreateError($"Could not set {name.ToString()} to value {value}", result.ErrorAppTag);
            throw new NetSchemaDataException(result.ToString());
        }

        private Result SetLeafList(IDataLeafList list, IEnumerable enumerable)
        {
            return DataEvaluator.TrySetValue(list, enumerable)
                ? Result.SuccessfulResult
                : Result.CreateError();
        }

        private bool TryGetProperty(QualifiedName qName, out object? value)
        {
            value = default;
            if (!this.container.TryFindOrCreate(qName, out var node))
                throw new NotImplementedException();
            if (node is not IDataLeaf leaf)
            {
                value = Create(node);
                return true;
            }
            throw new NotImplementedException();
        }
        
        private bool TryGetProperty(string? name, out object? value)
        {
            value = default;
            return TryGetQName(name, out var qName) && this.TryGetProperty(qName, out value);
        }


        public override bool TryGetMember(
            GetMemberBinder binder, 
            [NotNullWhen(true)] out object? result
        ) => TryGetProperty(binder.Name, out result);

        public override bool TrySetMember(
            SetMemberBinder binder,
            object? value
        ) => this.TrySetProperty(binder.Name, value);
    }
}