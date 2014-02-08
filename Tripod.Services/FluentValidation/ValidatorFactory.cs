using System;
using FluentValidation;
using SimpleInjector;

namespace Tripod.Services.FluentValidation
{
    public class ValidatorFactory : ValidatorFactoryBase
    {
        private readonly Container _container;

        public ValidatorFactory(Container container)
        {
            _container = container;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            return _container.GetInstance(validatorType) as IValidator;
        }
    }
}
