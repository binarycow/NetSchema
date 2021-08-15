using NetSchema.Data.Nodes;

namespace NetSchema.Data.Operations
{
    public sealed class EditConfigInput : RpcInput
    {
        public ConfigOperation DefaultOperation { get; init; }
        public TestOption TestOption { get; init; }
        public ErrorOption ErrorOption { get; init; }
        public IDataNode? EditContent { get; init; }
    }
}