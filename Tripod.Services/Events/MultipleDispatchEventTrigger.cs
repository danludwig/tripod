using System;
using System.Collections.Generic;
using System.Linq;
using SimpleInjector;

namespace Tripod.Services.Events
{
    public sealed class MultipleDispatchEventTrigger<TEvent> : ITriggerEvent<TEvent> where TEvent : IDefineEvent
    {
        private readonly Container _container;

        public MultipleDispatchEventTrigger(Container container)
        {
            _container = container;
        }

        public void Trigger(TEvent e)
        {
            IList<IHandleEvent<TEvent>> handlers = GetHandlers(_container);
            if (handlers != null && handlers.Any())
            {
                foreach (IHandleEvent<TEvent> handler in handlers)
                {
                    handler.Handle(e);
                }
            }
        }

        internal static IList<IHandleEvent<TEvent>> GetHandlers(Container container)
        {
            Type handlersType = typeof(IEnumerable<IHandleEvent<TEvent>>);
            IHandleEvent<TEvent>[] handlers = container.GetCurrentRegistrations()
                .Where(x => handlersType.IsAssignableFrom(x.ServiceType))
                .Select(x => x.GetInstance()).Cast<IEnumerable<IHandleEvent<TEvent>>>()
                .SelectMany(x =>
                {
                    var handleEvents = x as IHandleEvent<TEvent>[] ?? x.ToArray();
                    return handleEvents;
                })
                .ToArray()
            ;
            return handlers;
        }
    }
}
