using System;
using System.Security.Principal;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustFindUserByPrincipalExtensions
    {
        /// <summary>
        /// Validates that an User entity exists in the underlying data store for this authenticated Principal.
        /// </summary>
        /// <typeparam name="T">The command with the Principal to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating User by Principal.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, IPrincipal> MustFindUserByPrincipal<T>
            (this IRuleBuilder<T, IPrincipal> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindUserByPrincipal(queries));
        }
    }

    internal class MustFindUserByPrincipal : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindUserByPrincipal(IProcessQueries queries)
            : base(() => Resources.Validation_DoesNotExist)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var principal = (IPrincipal)context.PropertyValue;
            if (principal != null && principal.Identity.IsAuthenticated)
            {
                var entity = _queries.Execute(new UserBy(principal)).Result;
                if (entity != null) return true;
            }

            context.MessageFormatter.AppendArgument("PropertyValue", principal != null ? principal.Identity.Name : "");
            return false;
        }
    }
}
