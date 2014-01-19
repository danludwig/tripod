using System;
using System.Threading.Tasks;
using SimpleInjector;

namespace Tripod.Ioc.Net
{
    public class DeliverEmailAsynchronouslyDecorator : IDeliverEmailMessage
    {
        private readonly Container _container;
        private readonly Func<IDeliverEmailMessage> _factory;

        public DeliverEmailAsynchronouslyDecorator(Container container, Func<IDeliverEmailMessage> factory)
        {
            _factory = factory;
            _container = container;
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
