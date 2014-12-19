using System.Reflection;
using SimpleInjector;
using SimpleInjector.Extensions;

namespace Tripod.Services.Events
{
    public static class CompositionRoot
    {
        public static void RegisterEventing(this Container container, params Assembly[] assemblies)
        {
            assemblies = assemblies ?? new[] { Assembly.GetAssembly(typeof(IHandleEvent<>)), };

            container.RegisterSingle<IProcessEvents, EventProcessor>();

            container.RegisterManyForOpenGeneric(typeof(IHandleEvent<>), container.RegisterAll, assemblies);

            container.RegisterSingleOpenGeneric(typeof(ITriggerEvent<>), typeof(MultipleDispatchEventTrigger<>));
            container.RegisterDecorator(
                typeof(ITriggerEvent<>),
                typeof(TriggerEventWhenHandlersExistDecorator<>)
            );
        }
    }
}
