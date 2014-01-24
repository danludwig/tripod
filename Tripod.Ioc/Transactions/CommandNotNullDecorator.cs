using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Tripod.Ioc.Transactions
{
    public class CommandNotNullDecorator<TCommand> : IHandleCommand<TCommand> where TCommand : IDefineCommand
    {
        private readonly Func<IHandleCommand<TCommand>> _handlerFactory;

        public CommandNotNullDecorator(Func<IHandleCommand<TCommand>> handlerFactory)
        {
            _handlerFactory = handlerFactory;
        }

        [DebuggerStepThrough]
        public Task Handle(TCommand command)
        {
            if (Equals(command, null)) throw new ArgumentNullException("command");
            return _handlerFactory().Handle(command);
        }
    }
}