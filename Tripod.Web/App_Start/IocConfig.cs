using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;
using Tripod.Ioc;
using Tripod.Ioc.FluentValidation;

namespace Tripod.Web
{
    public static class IocConfig
    {
        public static void Configure()
        {
            var container = new Container();
            var settings = new RootCompositionSettings();
            container.ComposeRoot(settings);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());
            container.RegisterMvcAttributeFilterProvider();

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new WebApiDependencyResolver(container);

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