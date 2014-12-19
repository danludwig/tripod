using System;
using System.Threading.Tasks;

namespace Tripod.Services.Transactions
{
    public class CommandNotNullDecorator<TCommand> : IHandleCommand<TCommand> where TCommand : IDefineCommand
    {
        private readonly Func<IHandleCommand<TCommand>> _handlerFactory;

        public CommandNotNullDecorator(Func<IHandleCommand<TCommand>> handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public Task Handle(TCommand command)
        {
            if (Equals(command, null)) throw new ArgumentNullException("command");
            return _handlerFactory().Handle(command);
        }
    }
}