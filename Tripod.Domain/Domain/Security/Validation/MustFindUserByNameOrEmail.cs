using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustFindUserByNameOrEmailExtensions
    {
        public static IRuleBuilderOptions<T, string> MustFindUserByNameOrEmail<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindUserByNameOrEmail(queries));
        }
    }

    public class MustFindUserByNameOrEmail : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindUserByNameOrEmail(IProcessQueries queries)
            : base(() => Resources.Validation_CouldNotFind)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var nameOrEmail = (string)context.PropertyValue;
            if (string.IsNullOrWhiteSpace(nameOrEmail)) return false;

            var user = _queries.Execute(new UserByNameOrVerifiedEmail(nameOrEmail)).Result;
            if (user != null) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }
}
