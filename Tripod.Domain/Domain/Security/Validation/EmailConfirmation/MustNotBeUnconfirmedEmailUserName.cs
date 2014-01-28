using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotBeUnconfirmedEmailUserName<T> : PropertyValidator
    {
        private readonly Func<T, int> _userId;
        private readonly IProcessQueries _queries;

        internal MustNotBeUnconfirmedEmailUserName(Func<T, int> userId, IProcessQueries queries)
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

            var userId = _userId((T)context.Instance);
            var emailAddresses = _queries.Execute(new EmailAddressesBy(userId)
            {
                IsConfirmed = true,
            }).Result;
            var matchingAddress = emailAddresses.ByValueAsync(userName).Result;
            if (matchingAddress != null) return true;

            context.MessageFormatter.AppendArgument("PropertyName", User.Constraints.NameLabel.ToLower());
            return false;
        }
    }

    public static class MustNotBeUnconfirmedEmailUserNameExtensions
    {
        public static IRuleBuilderOptions<T, string> MustNotBeUnconfirmedEmailUserName<T>
            (this IRuleBuilder<T, string> ruleBuilder, Func<T, int> userId, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBeUnconfirmedEmailUserName<T>(userId, queries));
        }
    }
}
