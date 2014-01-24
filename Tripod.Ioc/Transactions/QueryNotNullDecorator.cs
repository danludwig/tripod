using System;
using System.Diagnostics;
using SimpleInjector;

namespace Tripod.Ioc.Transactions
{
    public class QueryNotNullDecorator<TQuery, TResult> : IHandleQuery<TQuery, TResult> where TQuery : IDefineQuery<TResult>
    {
        private readonly Container _container;
        private readonly Func<IHandleQuery<TQuery, TResult>> _handlerFactory;

        public QueryNotNullDecorator(Container container, Func<IHandleQuery<TQuery, TResult>> handlerFactory)
        {
            _container = container;
            _handlerFactory = handlerFactory;
        }

        [DebuggerStepThrough]
        public TResult Handle(TQuery query)
        {
            if (Equals(query, null)) throw new ArgumentNullException("query");
            return _handlerFactory().Handle(query);
        }
    }
}