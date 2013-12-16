using System.Diagnostics;
using System.Threading.Tasks;
using SimpleInjector;

namespace Tripod.Ioc.Transactions
{
    [UsedImplicitly]
    sealed class CommandProcessor : IProcessCommands
    {
        private readonly Container _container;

        public CommandProcessor(Container container)
        {
            _container = container;
        }

        [DebuggerStepThrough]
        public Task Execute(IDefineCommand command)
        {
            var handlerType = typeof(IHandleCommand<>).MakeGenericType(command.GetType());
            dynamic handler = _container.GetInstance(handlerType);
            return handler.Handle((dynamic)command);
        }
    }
}