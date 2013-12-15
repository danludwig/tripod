using System.Collections.Generic;

namespace Tripod
{
    public class FakeEntityWithCollectionProperty : EntityWithId<string>
    {
        public string Name { get; set; }
        public ICollection<FakeEntityWithSortableProperties> CollectionProperty { get; set; }
    }
}