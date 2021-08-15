#nullable enable

namespace NetSchema.Common
{
    internal interface IParent<in TParent>
    {
        TParent? Parent { set; }
    }
}