using System.Diagnostics;
using FluentValidation;
using FluentValidation.Results;
using SimpleInjector;

namespace Tripod.Ioc.FluentValidation
{
    [UsedImplicitly]
    sealed class ValidationProcessor : IProcessValidation
    {
        private readonly Container _container;

        public ValidationProcessor(Container container)
        {
            _container = container;
        }

        [DebuggerStepThrough]
        public ValidationResult Validate<TResult>(IDefineQuery<TResult> query)
        {
            var validatedType = typeof(IValidator<>).MakeGenericType(query.GetType());
            dynamic validator = _container.GetInstance(validatedType);
            return validator.Validate((dynamic)query);
        }

        [DebuggerStepThrough]
        public ValidationResult Validate(IDefineCommand command)
        {
            var validatedType = typeof(IValidator<>).MakeGenericType(command.GetType());
            dynamic validator = _container.GetInstance(validatedType);
            return validator.Validate((dynamic)command);
        }
    }
}
