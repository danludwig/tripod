using System.Linq;
using Should;
using SimpleInjector;
using Tripod.Ioc.FluentValidation;
using Xunit;

namespace Tripod.Ioc.Transactions
{
    public class CompositionRootTests : SimpleInjectorContainerTests
    {
        [Fact]
        public void RegistersIProcessQueries_UsingQueryProcessor_AsSingleton()
        {
            var instance = Container.GetInstance<IProcessQueries>();
            var registration = Container.GetRegistration(typeof(IProcessQueries));

            instance.ShouldNotBeNull();
            instance.ShouldBeType<QueryProcessor>();
            registration.Lifestyle.ShouldEqual(Lifestyle.Singleton);
        }

        [Fact]
        public void RegistersIHandleQuery_UsingOpenGenerics_WithDecorationChain()
        {
            var instance = Container.GetInstance<IHandleQuery<FakeQuery, string>>();
            var registration = Container.GetRegistration(typeof(IHandleQuery<FakeQuery, string>));

            instance.ShouldNotBeNull();
            registration.Registration.ImplementationType.ShouldEqual(typeof(HandleFakeQuery));
            registration.Registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            var decoratorChain = registration.GetRelationships()
                .Select(x => new
                {
                    x.ImplementationType,
                    x.Lifestyle,
                })
                .Reverse().Distinct().ToArray();
            decoratorChain.Length.ShouldEqual(2);
            decoratorChain[0].ImplementationType.ShouldEqual(typeof(QueryLifetimeScopeDecorator<FakeQuery, string>));
            decoratorChain[0].Lifestyle.ShouldEqual(Lifestyle.Singleton);
            decoratorChain[1].ImplementationType.ShouldEqual(typeof(ValidateQueryDecorator<FakeQuery, string>));
            decoratorChain[1].Lifestyle.ShouldEqual(Lifestyle.Transient);
        }

        [Fact]
        public void RegistersIProcessCommands_UsingCommandProcessor_AsSingleton()
        {
            var instance = Container.GetInstance<IProcessCommands>();
            var registration = Container.GetRegistration(typeof(IProcessCommands));

            instance.ShouldNotBeNull();
            instance.ShouldBeType<CommandProcessor>();
            registration.Lifestyle.ShouldEqual(Lifestyle.Singleton);
        }

        [Fact]
        public void RegistersIHandleCommand_UsingOpenGenerics_WithDecorationChain()
        {
            var instance = Container.GetInstance<IHandleCommand<FakeCommandWithValidator>>();
            var registration = Container.GetRegistration(typeof(IHandleCommand<FakeCommandWithValidator>));

            instance.ShouldNotBeNull();
            registration.Registration.ImplementationType.ShouldEqual(typeof(HandleFakeCommandWithValidator));
            registration.Registration.Lifestyle.ShouldEqual(Lifestyle.Transient);
            var decoratorChain = registration.GetRelationships()
                .Select(x => new
                {
                    x.ImplementationType,
                    x.Lifestyle,
                })
                .Reverse().Distinct().ToArray();
            decoratorChain.Length.ShouldEqual(2);
            decoratorChain[0].ImplementationType.ShouldEqual(typeof(CommandLifetimeScopeDecorator<FakeCommandWithValidator>));
            decoratorChain[0].Lifestyle.ShouldEqual(Lifestyle.Singleton);
            decoratorChain[1].ImplementationType.ShouldEqual(typeof(ValidateCommandDecorator<FakeCommandWithValidator>));
            decoratorChain[1].Lifestyle.ShouldEqual(Lifestyle.Transient);
        }
    }
}
