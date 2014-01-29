using System;
using System.Security.Principal;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBePrincipalWithUserId<T> : PropertyValidator
    {
        private readonly Func<T, int> _userId;

        internal MustBePrincipalWithUserId(Func<T, int> userId)
            : base(() => Resources.Validation_UserCommand_NotAuthorized)
        {
            if (userId == null) throw new ArgumentNullException("userId");
            _userId = userId;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var principal = (IPrincipal)context.PropertyValue;
            var userId = _userId((T)context.Instance);
            if (principal.Identity.GetAppUserId() == userId) return true;

            context.MessageFormatter.AppendArgument("PropertyValue", principal.Identity.Name);
            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            context.MessageFormatter.AppendArgument("UserId", userId);
            return false;
        }
    }

    public static class MustBePrincipalWithUserIdExtensions
    {
        public static IRuleBuilderOptions<T, IPrincipal> MustBePrincipalWithUserId<T>
            (this IRuleBuilder<T, IPrincipal> ruleBuilder, Func<T, int> userId)
        {
            return ruleBuilder.SetValidator(new MustBePrincipalWithUserId<T>(userId));
        }
    }
}
