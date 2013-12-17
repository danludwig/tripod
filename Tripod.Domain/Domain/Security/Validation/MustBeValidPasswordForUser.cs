using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBeVerifiedPassword<T> : PropertyValidator
    {
        private readonly Func<T, string> _userName;
        private readonly IProcessQueries _queries;

        internal MustBeVerifiedPassword(Func<T, string> userName, IProcessQueries queries)
            : base(() => Resources.Validation_InvalidUsernameOrPassword)
        {
            if (userName == null) throw new ArgumentNullException("userName");
            if (queries == null) throw new ArgumentNullException("queries");
            _userName = userName;
            _queries = queries;
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

            // assert that user does not exist
            return isVerified;
        }
    }

    public static class MustBeVerifiedPasswordExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBeVerifiedPassword<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries, Func<T, string> userName)
        {
            return ruleBuilder.SetValidator(new MustBeVerifiedPassword<T>(userName, queries));
        }
    }
}
