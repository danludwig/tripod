using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimpleInjector;

namespace Tripod.Services.Events
{
    public sealed class TriggerEventWhenHandlersExistDecorator<TEvent> : ITriggerEvent<TEvent> where TEvent : IDefineEvent
    {
        private readonly Container _container;
        private readonly Func<ITriggerEvent<TEvent>> _factory;

        public TriggerEventWhenHandlersExistDecorator(Container container, Func<ITriggerEvent<TEvent>> factory)
        {
            _container = container;
            _factory = factory;
        }

        public void Trigger(TEvent e)
        {
            // there is no need to start a new thread unless there are handlers registered for this event
            IList<IHandleEvent<TEvent>> handlers = MultipleDispatchEventTrigger<TEvent>.GetHandlers(_container);
            if (handlers != null && handlers.Any())
            {
                Task.Factory.StartNew(() =>
                {
                    if (_container.GetCurrentLifetimeScope() != null)
                    {
                        _factory().Trigger(e);
                    }
                    else
                    {
                        using (_container.BeginLifetimeScope())
                        {
                            _factory().Trigger(e);
                        }
                    }
                });
            }
        }
    }
}
