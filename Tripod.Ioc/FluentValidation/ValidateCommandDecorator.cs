using System.Diagnostics;
using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Ioc.FluentValidation
{
    public class ValidateCommandDecorator<TCommand> : IHandleCommand<TCommand> where TCommand : IDefineCommand
    {
        private readonly IHandleCommand<TCommand> _decorated;
        private readonly IValidator<TCommand> _validator;

        public ValidateCommandDecorator(IHandleCommand<TCommand> decorated
            , IValidator<TCommand> validator
        )
        {
            _decorated = decorated;
            _validator = validator;
        }

        [DebuggerStepThrough]
        public Task Handle(TCommand command)
        {
            _validator.ValidateAndThrow(command);

            return _decorated.Handle(command);
        }
    }
}
