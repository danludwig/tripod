using System;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotBeUnverifiedEmailUserName<T> : PropertyValidator
    {
        private readonly Func<T, string> _token;
        private readonly Func<T, int> _userId;
        private readonly IProcessQueries _queries;

        internal MustNotBeUnverifiedEmailUserName(Func<T, string> token, IProcessQueries queries)
            : base(() => Resources.Validation_UserName_AllowedEmailAddress)
        {
            if (token == null) throw new ArgumentNullException("token");
            if (queries == null) throw new ArgumentNullException("queries");
            _token = token;
            _queries = queries;
        }

        internal MustNotBeUnverifiedEmailUserName(Func<T, int> userId, IProcessQueries queries)
            : base(() => Resources.Validation_UserName_AllowedEmailAddress)
        {
            if (userId == null) throw new ArgumentNullException("userId");
            if (queries == null) throw new ArgumentNullException("queries");
            _userId = userId;
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var userName = (string)context.PropertyValue;
            if (string.IsNullOrWhiteSpace(userName)) return true;

            if (!EmailAddress.ValueRegex.IsMatch(userName)) return true;

            if (_token != null)
            {
                var token = _token((T) context.Instance);
                var userToken = _queries.Execute(new EmailVerificationUserToken(token)).Result;
                if (userToken == null) return true;
                var verification = _queries.Execute(new EmailVerificationBy(userToken.Value)
                {
                    EagerLoad = new Expression<Func<EmailVerification, object>>[]
                    {
                        x => x.EmailAddress,
                    },
                }).Result;
                if (verification == null) return true;
                if (verification.EmailAddress.Value.Equals(userName, StringComparison.OrdinalIgnoreCase)) return true;
            }
            else
            {
                var userId = _userId((T)context.Instance);
                var emailAddresses = _queries.Execute(new EmailAddressesBy(userId)
                {
                    IsVerified = true,
                }).Result;
                var matchingAddress = emailAddresses.ByValueAsync(userName).Result;
                if (matchingAddress != null) return true;
            }

            context.MessageFormatter.AppendArgument("PropertyName", User.Constraints.NameLabel.ToLower());
            return false;
        }
    }

    public static class MustNotBeUnverifiedEmailUserNameExtensions
    {
        public static IRuleBuilderOptions<T, string> MustNotBeUnverifiedEmailUserName<T>
            (this IRuleBuilder<T, string> ruleBuilder, Func<T, string> token, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBeUnverifiedEmailUserName<T>(token, queries));
        }

        public static IRuleBuilderOptions<T, string> MustNotBeUnverifiedEmailUserName<T>
            (this IRuleBuilder<T, string> ruleBuilder, Func<T, int> userId, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBeUnverifiedEmailUserName<T>(userId, queries));
        }
    }
}
