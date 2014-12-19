using System;
using SimpleInjector;

namespace Tripod.Services.Events
{
    [UsedImplicitly]
    internal sealed class EventProcessor : IProcessEvents
    {
        private readonly Container _container;

        public EventProcessor(Container container)
        {
            _container = container;
        }

        [System.Diagnostics.DebuggerStepThrough]
        public void Process(IDefineEvent e)
        {
            Type triggerType = typeof(ITriggerEvent<>).MakeGenericType(e.GetType());
            dynamic trigger = _container.GetInstance(triggerType);
            trigger.Trigger((dynamic)e);
        }
    }
}
