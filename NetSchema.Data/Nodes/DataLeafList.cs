#nullable enable

using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Resolve.Nodes;
using NetSchema.Types;

namespace NetSchema.Data.Nodes
{
    public interface IDataLeafList : IDataNode
    {
        IUsableType Type { get; }
        Result TryAddValue(object? value);
        Result TryAddValue(string value);
        IReadOnlyList<string> Values { get; }
        void Clear();
    }
    
    internal class DataLeafList : DataNode<IResolvedSchemaLeafList>, IDataLeafList
    {
        public DataLeafList(IDataObject parent, IResolvedSchemaLeafList schemaNode) : base(parent, schemaNode)
        {
        }
        public IUsableType Type => this.TypedSchemaNode.Type;
        
        public Result TryAddValue(string value)
        {
            if (!this.Type.ValidateAndGetCanonicalValue(value).Try(out var canonical, out var error))
                return error;
            this._Values.Add(canonical);
            return Result.SuccessfulResult;
        }

        private readonly List<string> _Values = new ();
        public IReadOnlyList<string> Values => this._Values.AsReadOnly();
        public void Clear() => this._Values.Clear();

        public Result TryAddValue(object? value) => this.TryAddValue(value?.ToString() ?? string.Empty);
    }
}