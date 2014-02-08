namespace Tripod.Services.Transactions
{
    public class FakeQueryWithoutValidator : IDefineQuery<string> { }

    public class HandleFakeQueryWithoutValidator : IHandleQuery<FakeQueryWithoutValidator, string>
    {
        public string Handle(FakeQueryWithoutValidator query)
        {
            return "faked";
        }
    }
}