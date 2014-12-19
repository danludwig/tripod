using System.Reflection;
using SimpleInjector;
using SimpleInjector.Extensions;
using Tripod.Services.FluentValidation;

namespace Tripod.Services.Transactions
{
    public static class CompositionRoot
    {
        public static void RegisterQueryTransactions(this Container container, params Assembly[] assemblies)
        {
            assemblies = assemblies ?? new[] { Assembly.GetAssembly(typeof(IHandleQuery<,>)), };

            container.RegisterSingle<IProcessQueries, QueryProcessor>();

            container.RegisterManyForOpenGeneric(typeof(IHandleQuery<,>), assemblies);
            container.RegisterDecorator(
                typeof(IHandleQuery<,>),
                typeof(ValidateQueryDecorator<,>)
            );
            container.RegisterSingleDecorator(
                typeof(IHandleQuery<,>),
                typeof(QueryLifetimeScopeDecorator<,>)
            );
            container.RegisterSingleDecorator(
                typeof(IHandleQuery<,>),
                typeof(QueryNotNullDecorator<,>)
            );
        }

        public static void RegisterCommandTransactions(this Container container, params Assembly[] assemblies)
        {
            assemblies = assemblies ?? new[] { Assembly.GetAssembly(typeof(IHandleCommand<>)), };

            container.RegisterSingle<IProcessCommands, CommandProcessor>();

            container.RegisterManyForOpenGeneric(typeof(IHandleCommand<>), assemblies);
            container.RegisterDecorator(
                typeof(IHandleCommand<>),
                typeof(CommandedEventProcessingDecorator<>)
            );
            container.RegisterDecorator(
                typeof(IHandleCommand<>),
                typeof(ValidateCommandDecorator<>)
            );
            container.RegisterSingleDecorator(
                typeof(IHandleCommand<>),
                typeof(CommandLifetimeScopeDecorator<>)
            );
            container.RegisterSingleDecorator(
                typeof(IHandleCommand<>),
                typeof(CommandNotNullDecorator<>)
            );
        }
    }
}
