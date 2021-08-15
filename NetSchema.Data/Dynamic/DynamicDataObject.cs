using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq.Expressions;
using NetSchema.Common;
using NetSchema.Data.Nodes;

namespace NetSchema.Data.Dynamic
{
    internal abstract class DynamicDataObject : DynamicObject
    {
        private readonly IDataNode dataObject;

        protected DynamicDataObject(IDataNode dataObject)
        {
            this.dataObject = dataObject;
        }
        public static DynamicDataObject Create(IDataNode dataObject) => dataObject switch
        {
            IContainerLikeDataNode container => new DynamicContainer(container),
            IDataUnkeyedList list => new DynamicList(list),
            IDataKeyedList list => new DynamicDictionary(list),
            _ => throw new NotImplementedException(),
        };
        

        protected bool TryGetQName(IReadOnlyList<object> indexes, out QualifiedName qName)
        {
            if (indexes.Count != 1 || indexes[0] is not string name)
                throw new NotImplementedException();
            return TryGetQName(name, out qName);
        }
        protected bool TryGetQName(string? name, out QualifiedName qName)
        {
            if (name is null || QualifiedName.TryParse(name, out qName) == false)
                return false;
            qName = DataSerializer.HandleDefaultModuleNames(qName, this.dataObject);
            return true;
        }


        public abstract override bool TryGetIndex(
            GetIndexBinder binder, 
            object[] indexes,
            [NotNullWhen(true)]  out object? result
        );
        public abstract override bool TrySetIndex(
            SetIndexBinder binder, 
            object[] indexes, 
            object? value
        );
        public abstract override bool TryGetMember(
            GetMemberBinder binder,
            [NotNullWhen(true)] out object? result
        );
        public abstract override bool TrySetMember(
            SetMemberBinder binder,
            object? value
        );

        
        
        
        
        public override IEnumerable<string> GetDynamicMemberNames() => throw new NotImplementedException();

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result) => throw new NotImplementedException();

        public override bool TryConvert(ConvertBinder binder, out object result) => throw new NotImplementedException();

        public override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, out object result) => throw new NotImplementedException();

        public override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes) => throw new NotImplementedException();

        public override bool TryDeleteMember(DeleteMemberBinder binder) => throw new NotImplementedException();


        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result) => throw new NotImplementedException();

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result) => throw new NotImplementedException();

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result) => throw new NotImplementedException();
        public override DynamicMetaObject GetMetaObject(Expression parameter) => base.GetMetaObject(parameter);
    }
    

}