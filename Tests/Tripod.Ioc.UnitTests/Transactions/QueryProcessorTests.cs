using System.Reflection;
using Should;
using SimpleInjector;
using SimpleInjector.Extensions;
using Xunit;

namespace Tripod.Ioc.Transactions
{
    public class QueryProcessorTests
    {
        [Fact]
        public void Execute_InvokesQueryHandler_UsingContainerForResolution()
        {
            var container = new Container();
            container.RegisterSingle<IProcessQuery, QueryProcessor>();
            container.RegisterManyForOpenGeneric(typeof(IHandleQuery<,>), Assembly.GetAssembly(GetType()));
            container.Verify();
            var queryProcessor = container.GetInstance<IProcessQuery>();
            var result = queryProcessor.Execute(new FakeQuery());
            result.ShouldEqual("faked");
        }
    }
}
