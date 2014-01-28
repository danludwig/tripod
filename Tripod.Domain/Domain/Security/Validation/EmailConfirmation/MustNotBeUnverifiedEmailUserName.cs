using System;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotBeUnverifiedEmailUserName<T> : PropertyValidator
    {
        private readonly Func<T, string> _token;
        private readonly IProcessQueries _queries;

        internal MustNotBeUnverifiedEmailUserName(Func<T, string> token, IProcessQueries queries)
            : base(() => Resources.Validation_UserName_AllowedEmailAddress)
        {
            if (token == null) throw new ArgumentNullException("token");
            if (queries == null) throw new ArgumentNullException("queries");
            _token = token;
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var userName = (string)context.PropertyValue;
            if (string.IsNullOrWhiteSpace(userName)) return true;

            if (!EmailAddress.ValueRegex.IsMatch(userName)) return true;

            var token = _token((T)context.Instance);
            var userToken = _queries.Execute(new EmailConfirmationUserToken(token)).Result;
            if (userToken == null) return true;
            var confirmation = _queries.Execute(new EmailConfirmationBy(userToken.Value)
            {
                EagerLoad = new Expression<Func<EmailConfirmation, object>>[]
                {
                    x => x.Owner,
                },
            }).Result;
            if (confirmation == null) return true;
            if (confirmation.Owner.Value.Equals(userName, StringComparison.OrdinalIgnoreCase)) return true;

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
    }
}
