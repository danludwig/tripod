using System;

namespace Tripod
{
    internal class FakeEntityWithGuidId : EntityWithId<Guid>
    {
        public FakeEntityWithGuidId(Guid id)
        {
            Id = id;
        }
    }
}