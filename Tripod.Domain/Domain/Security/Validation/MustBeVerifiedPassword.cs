using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBeVerifiedPassword<T> : PropertyValidator
    {
        private readonly IProcessQueries _queries;
        private readonly Func<T, string> _userName;

        internal MustBeVerifiedPassword(IProcessQueries queries, Func<T, string> userName)
            : base(() => Resources.Validation_InvalidUsernameOrPassword)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            if (userName == null) throw new ArgumentNullException("userName");
            _queries = queries;
            _userName = userName;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var password = (string)context.PropertyValue;
            var userName = _userName((T)context.Instance);
            var query = new IsPasswordVerified
            {
                UserName = userName,
                Password = password,
            };
            var isVerified = _queries.Execute(query).Result;

            return isVerified;
        }
    }

    public static class MustBeVerifiedPasswordExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeVerifiedPassword<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries, Func<T, string> userName)
        {
            return ruleBuilder.SetValidator(new MustBeVerifiedPassword<T>(queries, userName));
        }
    }
}
