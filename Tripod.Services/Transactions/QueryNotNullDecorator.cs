using System;

namespace Tripod.Services.Transactions
{
    public class QueryNotNullDecorator<TQuery, TResult> : IHandleQuery<TQuery, TResult> where TQuery : IDefineQuery<TResult>
    {
        private readonly Func<IHandleQuery<TQuery, TResult>> _handlerFactory;

        public QueryNotNullDecorator(Func<IHandleQuery<TQuery, TResult>> handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public TResult Handle(TQuery query)
        {
            if (Equals(query, null)) throw new ArgumentNullException("query");
            return _handlerFactory().Handle(query);
        }
    }
}