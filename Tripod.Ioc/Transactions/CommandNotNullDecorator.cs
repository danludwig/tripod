using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SimpleInjector;

namespace Tripod.Ioc.Transactions
{
    public class CommandNotNullDecorator<TCommand> : IHandleCommand<TCommand> where TCommand : IDefineCommand
    {
        private readonly Container _container;
        private readonly Func<IHandleCommand<TCommand>> _handlerFactory;

        public CommandNotNullDecorator(Container container, Func<IHandleCommand<TCommand>> handlerFactory)
        {
            _container = container;
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