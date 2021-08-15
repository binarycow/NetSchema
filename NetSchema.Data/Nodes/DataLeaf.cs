#nullable enable

using System.Xml.Linq;
using NetSchema.Common;
using NetSchema.Resolve.Nodes;
using NetSchema.Types;

namespace NetSchema.Data.Nodes
{
    public interface IDataLeaf : IDataNode
    {
        IUsableType Type { get; }
        Result SetValue(object? value);
        Result SetValue(string value);
        
        string Value { get; }
    }
    
    internal class DataLeaf : DataNode<IResolvedSchemaLeaf>, IDataLeaf
    {
        public DataLeaf(IDataObject parent, IResolvedSchemaLeaf schemaNode) : base(parent, schemaNode)
        {
        }
        public string Value { get; private set; } = string.Empty;

        public IUsableType Type => this.TypedSchemaNode.Type;
        public Result SetValue(object? value) => this.SetValue(value?.ToString() ?? string.Empty);

        public Result SetValue(string value)
        {
            if (!this.Type.ValidateAndGetCanonicalValue(value).Try(out var canonical, out var error))
                return error;
            this.Value = canonical;
            return Result.SuccessfulResult;
        }
    }
}