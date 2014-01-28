using System;
using System.Security.Principal;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class MustBePrincipalWithUserId<T> : PropertyValidator
    {
        private readonly IProcessQueries _queries;
        private readonly Func<T, int> _userId;

        internal MustBePrincipalWithUserId(IProcessQueries queries, Func<T, int> userId)
            : base(() => Resources.Validation_UserCommand_NotAuthorized)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            if (userId == null) throw new ArgumentNullException("userId");
            _queries = queries;
            _userId = userId;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var principal = (IPrincipal)context.PropertyValue;
            var userId = _userId((T)context.Instance);
            if (int.Parse(principal.Identity.GetUserId()) == userId) return true;

            context.MessageFormatter.AppendArgument("PropertyValue", principal.Identity.Name);
            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            context.MessageFormatter.AppendArgument("UserId", userId);
            return false;
        }
    }

    public static class MustBePrincipalWithUserIdExtensions
    {
        public static IRuleBuilderOptions<T, IPrincipal> MustBePrincipalWithUserId<T>
            (this IRuleBuilder<T, IPrincipal> ruleBuilder, IProcessQueries queries, Func<T, int> userId)
        {
            return ruleBuilder.SetValidator(new MustBePrincipalWithUserId<T>(queries, userId));
        }
    }
}
