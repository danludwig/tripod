namespace Tripod
{
    public class FakeEntityWithNavigationProperty : EntityWithId<int>
    {
        public FakeEntityWithSortableProperties NavigationProperty { get; set; }
    }
}
