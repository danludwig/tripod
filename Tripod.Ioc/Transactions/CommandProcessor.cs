using System.Diagnostics;
using SimpleInjector;

namespace Tripod.Ioc.Transactions
{
    sealed class CommandProcessor : IProcessCommands
    {
        private readonly Container _container;

        public CommandProcessor(Container container)
        {
            _container = container;
        }

        [DebuggerStepThrough]
        public void Execute(IDefineCommand command)
        {
            var handlerType = typeof(IHandleCommand<>).MakeGenericType(command.GetType());
            dynamic handler = _container.GetInstance(handlerType);
            handler.Handle((dynamic)command);
        }
    }
}