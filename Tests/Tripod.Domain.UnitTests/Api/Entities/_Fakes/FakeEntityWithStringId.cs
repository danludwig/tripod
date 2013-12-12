namespace Tripod
{
    internal class FakeEntityWithStringId : EntityWithId<string>
    {
        public FakeEntityWithStringId(string id)
        {
            Id = id;
        }
    }
}