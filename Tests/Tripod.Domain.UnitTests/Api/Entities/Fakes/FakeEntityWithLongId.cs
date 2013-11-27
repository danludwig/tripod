namespace Tripod
{
    internal class FakeEntityWithLongId : EntityWithId<long>
    {
        public FakeEntityWithLongId(long id)
        {
            Id = id;
        }
    }
}