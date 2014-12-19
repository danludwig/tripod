using System.Threading.Tasks;
using FluentValidation;

namespace Tripod.Services.FluentValidation
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

        [System.Diagnostics.DebuggerStepThrough]
        public Task Handle(TCommand command)
        {
            _validator.ValidateAndThrow(command);

            return _decorated.Handle(command);
        }
    }
}
