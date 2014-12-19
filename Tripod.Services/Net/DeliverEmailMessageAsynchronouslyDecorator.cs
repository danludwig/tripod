using System;
using System.Threading.Tasks;
using SimpleInjector;

namespace Tripod.Services.Net
{
    public class DeliverEmailAsynchronouslyDecorator : IDeliverEmailMessage
    {
        private readonly Container _container;
        private readonly Func<IDeliverEmailMessage> _factory;

        public DeliverEmailAsynchronouslyDecorator(Container container, Func<IDeliverEmailMessage> factory)
        {
            _container = container;
            _factory = factory;
        }

        public void Deliver(int emailMessageId)
        {
            Task.Factory.StartNew(() =>
            {
                using (_container.BeginLifetimeScope())
                    _factory().Deliver(emailMessageId);
            });
        }
    }
}
