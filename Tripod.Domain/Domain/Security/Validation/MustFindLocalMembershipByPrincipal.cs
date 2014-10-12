using System;
using System.Security.Principal;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class MustFindLocalMembershipByPrincipal : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindLocalMembershipByPrincipal(IProcessQueries queries)
            : base(() => Resources.Validation_LocalMembershipByUser_DoesNotExist)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var principal = (IPrincipal)context.PropertyValue;
            if (principal != null)
            {
                if (principal.Identity.HasAppUserId())
                {
                    var entity = _queries.Execute(new LocalMembershipByUser(principal.Identity.GetUserId<int>())).Result;
                    if (entity != null) return true;
                }
            }

            context.MessageFormatter.AppendArgument("PropertyValue", principal != null ? principal.Identity.Name : "");
            context.MessageFormatter.AppendArgument("PasswordLabel", LocalMembership.Constraints.Label.ToLower());
            return false;
        }
    }

    public static class MustFindLocalMembershipByPrincipalExtensions
    {
        public static IRuleBuilderOptions<T, IPrincipal> MustFindLocalMembershipByPrincipal<T>
            (this IRuleBuilder<T, IPrincipal> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindLocalMembershipByPrincipal(queries));
        }
    }
}
