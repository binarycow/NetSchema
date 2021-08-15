using System.Collections.Generic;
using System.Linq;

namespace NetSchema.Common.Collections
{
    internal abstract class DiverseChildCollection<TChild>
    {
        public IReadOnlyList<TChild> AllChildren { get; }

        protected DiverseChildCollection(IEnumerable<TChild> children) 
            => this.AllChildren = children.ToList().AsReadOnly();

        protected TSpecific? Update<TSpecific>(ref TSpecific? item)
            where TSpecific : TChild
        {
            item ??= this.AllChildren.OfType<TSpecific>().FirstOrDefault();
            return item;
        }
        
        protected IReadOnlyList<TSpecific> Update<TSpecific>(
            ref IReadOnlyList<TSpecific>? dataNodes
        ) where TSpecific : TChild
        {
            dataNodes ??= this.AllChildren.OfType<TSpecific>().ToList().AsReadOnly();
            return dataNodes;
        }
        
        protected IReadOnlyKeyedCollection<TSpecific> Update<TSpecific>(
            ref IReadOnlyKeyedCollection<TSpecific>? dataNodes
        ) where TSpecific : IReadOnlyNamedNode, TChild
        {
            dataNodes ??= new NamedNodeCollection<TSpecific>(
                this.AllChildren.OfType<TSpecific>()
            ).AsReadOnly();
            return dataNodes;
        }
    }
}