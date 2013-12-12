namespace Tripod.Ioc.Transactions
{
    public class FakeQuery : IDefineQuery<string> { }

    public class HandleFakeQuery : IHandleQuery<FakeQuery, string>
    {
        public string Handle(FakeQuery query)
        {
            return "faked";
        }
    }
}