namespace Tripod
{
    public class FakeClosedGenericB1 : IFakeOpenGenericB<int>
    {
        public int Property { get; [UsedImplicitly] set; }
    }
}