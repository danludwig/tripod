using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotFindUserByName : PropertyValidator
    {
        private readonly IProcessQuery _queryProcessor;

        internal MustNotFindUserByName(IProcessQuery queryProcessor)
            : base("{PropertyName} '{PropertyValue}' already exists.")
        {
            if (queryProcessor == null) throw new ArgumentNullException("queryProcessor");
            _queryProcessor = queryProcessor;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var userName = (string)context.PropertyValue;
            var entity = _queryProcessor.Execute(new UserBy(userName)).Result;

            // assert that user does not exist
            return entity == null;
        }
    }

    public static class MustNotFindUserByNameExtensions
    {
        public static IRuleBuilderOptions<T, string> MustNotFindUserByName<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQuery queryProcessor)
        {
            return ruleBuilder.SetValidator(new MustNotFindUserByName(queryProcessor));
        }
    }
}
