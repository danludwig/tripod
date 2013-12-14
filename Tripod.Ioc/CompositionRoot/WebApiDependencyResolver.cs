using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;
using SimpleInjector;

namespace Tripod.Ioc
{
    public class WebApiDependencyResolver : IDependencyResolver
    {
        private readonly Container _container;
        private readonly IServiceProvider _provider;

        public WebApiDependencyResolver(Container container)
        {
            if (container == null)
                throw new ArgumentNullException("container");
            _container = container;
            _provider = container;
        }

        public object GetService(Type serviceType)
        {
            return _provider.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.GetAllInstances(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            // This implementation does not support child scopes, so we simply return 'this'.
            return this;
        }

        public void Dispose()
        {
            // When BeginScope returns 'this', the Dispose method must be a no-op.
        }
    }
}
