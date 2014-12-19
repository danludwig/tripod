using System;
using System.Threading.Tasks;

namespace Tripod.Services.Transactions
{
    public class CommandedEventProcessingDecorator<TCommand> : IHandleCommand<TCommand> where TCommand : IDefineCommand
    {
        private readonly IProcessEvents _events;
        private readonly Func<IHandleCommand<TCommand>> _handlerFactory;

        public CommandedEventProcessingDecorator(IProcessEvents events, Func<IHandleCommand<TCommand>> handlerFactory)
        {
            _events = events;
            _handlerFactory = handlerFactory;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public Task Handle(TCommand command)
        {
            // this will handle the command as normal, and then will try to process it as an event
            // but only when the command is a BaseEntityCommand that has just been committed.
            var handler = _handlerFactory();
            var task = handler.Handle(command);
            var baseEntityCommand = command as BaseEntityCommand;
            if (baseEntityCommand != null && baseEntityCommand.Commit)
            {
                _events.Process((IDefineEvent)command);
            }
            return task;
        }
    }
}