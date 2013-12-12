using System;
using System.Diagnostics;
using SimpleInjector;

namespace Tripod.Ioc.Transactions
{
    public class QueryLifetimeScopeDecorator<TQuery, TResult> : IHandleQuery<TQuery, TResult> where TQuery : IDefineQuery<TResult>
    {
        private readonly Container _container;
        private readonly Func<IHandleQuery<TQuery, TResult>> _handlerFactory;

        public QueryLifetimeScopeDecorator(Container container, Func<IHandleQuery<TQuery, TResult>> handlerFactory)
        {
            _container = container;
            _handlerFactory = handlerFactory;
        }

        [DebuggerStepThrough]
        public TResult Handle(TQuery query)
        {
            if (_container.GetCurrentLifetimeScope() != null)
                return _handlerFactory().Handle(query);
            using (_container.BeginLifetimeScope())
                return _handlerFactory().Handle(query);
        }
    }
}
