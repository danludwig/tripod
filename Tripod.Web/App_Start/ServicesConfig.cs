using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using Tripod.Services;
using Tripod.Services.FluentValidation;

namespace Tripod.Web
{
    public static class ServicesConfig
    {
        public static void Configure()
        {
            var container = new Container();
            var settings = new RootCompositionSettings
            {
#if DEBUG
                IsGreenfield = true,
#endif
                FluentValidatorAssemblies = new[]
                {
                    Assembly.GetAssembly(typeof(IHandleCommand<>)),
                    Assembly.GetExecutingAssembly(),
                },

                EventHandlerAssemblies = new[]
                {
                    Assembly.GetAssembly(typeof(IHandleEvent<>)),
                    Assembly.GetExecutingAssembly(),
                },
            };
            container.ComposeRoot(settings);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterMvcAttributeFilterProvider();

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new WebApiDependencyResolver(container);
            GlobalHost.DependencyResolver.Register(typeof(IHubActivator), () => new HubActivator(container));

            FluentValidation.Mvc.FluentValidationModelValidatorProvider.Configure(
                provider =>
                {
                    provider.ValidatorFactory = new ValidatorFactory(container);
                    provider.AddImplicitRequiredValidator = false;
                }
            );
            FluentValidation.Mvc.WebApi.FluentValidationModelValidatorProvider.Configure(provider =>
            {
                provider.ValidatorFactory = new ValidatorFactory(container);
            });
        }
    }
}