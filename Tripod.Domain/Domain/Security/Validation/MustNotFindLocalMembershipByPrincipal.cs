using System;
using System.Security.Principal;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.AspNet.Identity;

namespace Tripod.Domain.Security
{
    public class MustNotFindLocalMembershipByPrincipal : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustNotFindLocalMembershipByPrincipal(IProcessQueries queries)
            : base(() => Resources.Validation_LocalMembershipByUser_AlreadyExists)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var principal = (IPrincipal)context.PropertyValue;
            var entity = _queries.Execute(new LocalMembershipByUser(principal.Identity.GetUserId())).Result;

            if (entity == null) return true;

            context.MessageFormatter.AppendArgument("PropertyValue", principal.Identity.Name);
            context.MessageFormatter.AppendArgument("PasswordLabel", LocalMembership.Constraints.Label.ToLower());
            return false;
        }
    }

    public static class MustNotFindLocalMembershipByPrincipalExtensions
    {
        public static IRuleBuilderOptions<T, IPrincipal> MustNotFindLocalMembershipByPrincipal<T>
            (this IRuleBuilder<T, IPrincipal> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotFindLocalMembershipByPrincipal(queries));
        }
    }
}
