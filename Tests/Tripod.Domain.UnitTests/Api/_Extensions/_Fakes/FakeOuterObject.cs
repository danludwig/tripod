namespace Tripod
{
    public class FakeOuterObject
    {
        public object Method() { return Inner; }

        public FakeInnerObject Inner { get; [UsedImplicitly] set; }

        [UsedImplicitly]
        public class FakeInnerObject
        {
            public int Property { get; [UsedImplicitly] set; }
        }
    }
}
