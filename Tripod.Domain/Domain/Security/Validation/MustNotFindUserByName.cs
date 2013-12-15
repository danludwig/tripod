using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotFindUserByName : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustNotFindUserByName(IProcessQueries queries)
            : base("{PropertyName} '{PropertyValue}' already exists.")
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var userName = (string)context.PropertyValue;
            var entity = _queries.Execute(new UserBy(userName)).Result;

            // assert that user does not exist
            return entity == null;
        }
    }

    public static class MustNotFindUserByNameExtensions
    {
        public static IRuleBuilderOptions<T, string> MustNotFindUserByName<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotFindUserByName(queries));
        }
    }
}
