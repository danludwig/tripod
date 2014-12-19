using System;
using Microsoft.AspNet.SignalR.Hubs;
using SimpleInjector;

namespace Tripod.Services
{
    public class HubActivator : IHubActivator
    {
        private readonly Container _container;

        public HubActivator(Container container)
        {
            if (container == null) throw new ArgumentNullException("container");
            _container = container;
        }

        public IHub Create(HubDescriptor descriptor)
        {
            return _container.GetInstance(descriptor.HubType) as IHub;
        }
    }
}
