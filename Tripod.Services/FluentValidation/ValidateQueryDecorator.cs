using FluentValidation;

namespace Tripod.Services.FluentValidation
{
    public class ValidateQueryDecorator<TQuery, TResult> : IHandleQuery<TQuery, TResult> where TQuery : IDefineQuery<TResult>
    {
        private readonly IHandleQuery<TQuery, TResult> _decorated;
        private readonly IValidator<TQuery> _validator;

        public ValidateQueryDecorator(IHandleQuery<TQuery, TResult> decorated
            , IValidator<TQuery> validator
        )
        {
            _decorated = decorated;
            _validator = validator;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public TResult Handle(TQuery query)
        {
            _validator.ValidateAndThrow(query);

            return _decorated.Handle(query);
        }
    }
}
