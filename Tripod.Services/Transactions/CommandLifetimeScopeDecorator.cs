using System;
using System.Threading.Tasks;
using SimpleInjector;

namespace Tripod.Services.Transactions
{
    public class CommandLifetimeScopeDecorator<TCommand> : IHandleCommand<TCommand> where TCommand : IDefineCommand
    {
        private readonly Container _container;
        private readonly Func<IHandleCommand<TCommand>> _handlerFactory;

        public CommandLifetimeScopeDecorator(Container container, Func<IHandleCommand<TCommand>> handlerFactory)
        {
            _container = container;
            _handlerFactory = handlerFactory;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public Task Handle(TCommand command)
        {
            if (_container.GetCurrentLifetimeScope() != null)
                return _handlerFactory().Handle(command);
            using (_container.BeginLifetimeScope())
                return _handlerFactory().Handle(command);
        }
    }
}
