using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustFindUserByVerifiedEmail : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindUserByVerifiedEmail(IProcessQueries queries)
            : base(() => Resources.Validation_CouldNotFind)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var email = (string)context.PropertyValue;
            if (string.IsNullOrWhiteSpace(email)) return false;

            var emailByValue = _queries.Execute(new EmailAddressBy(email)
            {
                IsVerified = true,
            }).Result;
            if (emailByValue != null) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }

    public static class MustFindUserByVerifiedEmailExtensions
    {
        public static IRuleBuilderOptions<T, string> MustFindUserByVerifiedEmail<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindUserByVerifiedEmail(queries));
        }
    }
}
