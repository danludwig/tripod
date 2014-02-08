using System;
using System.Threading.Tasks;
using SimpleInjector;

namespace Tripod.Services.Net
{
    public class DeliveredEmailAsynchronouslyDecorator : IDeliveredEmailMessage
    {
        private readonly Container _container;
        private readonly Func<IDeliveredEmailMessage> _factory;

        public DeliveredEmailAsynchronouslyDecorator(Container container, Func<IDeliveredEmailMessage> factory)
        {
            _factory = factory;
            _container = container;
        }

        public void OnDelivered(int emailMessageId, Exception error, bool cancelled)
        {
            Task.Factory.StartNew(() =>
            {
                using (_container.BeginLifetimeScope())
                    _factory().OnDelivered(emailMessageId, error, cancelled);
            });
        }
    }
}