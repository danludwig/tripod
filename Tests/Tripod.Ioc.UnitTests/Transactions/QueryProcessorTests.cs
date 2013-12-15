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
            container.RegisterSingle<IProcessQueries, QueryProcessor>();
            container.RegisterManyForOpenGeneric(typeof(IHandleQuery<,>), Assembly.GetExecutingAssembly());
            container.Verify();
            var queries = container.GetInstance<IProcessQueries>();
            var result = queries.Execute(new FakeQueryWithoutValidator());
            result.ShouldEqual("faked");
        }
    }
}
