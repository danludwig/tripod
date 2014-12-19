using SimpleInjector;

namespace Tripod.Services.Transactions
{
    [UsedImplicitly]
    internal sealed class QueryProcessor : IProcessQueries
    {
        private readonly Container _container;

        public QueryProcessor(Container container)
        {
            _container = container;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public TResult Execute<TResult>(IDefineQuery<TResult> query)
        {
            var handlerType = typeof(IHandleQuery<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic handler = _container.GetInstance(handlerType);
            return handler.Handle((dynamic)query);
        }
    }
}
