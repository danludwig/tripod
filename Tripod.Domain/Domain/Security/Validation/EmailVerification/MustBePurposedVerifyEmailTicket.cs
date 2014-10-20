using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustBePurposedVerifyEmailTicketExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBePurposedVerifyEmailTicket<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries, params Func<T, EmailVerificationPurpose>[] purposes)
        {
            return ruleBuilder.SetValidator(new MustBePurposedVerifyEmailTicket<T>(queries, purposes));
        }
    }

    public class MustBePurposedVerifyEmailTicket<T> : PropertyValidator
    {
        private readonly IProcessQueries _queries;
        private readonly Func<T, EmailVerificationPurpose>[] _purposes;

        internal MustBePurposedVerifyEmailTicket(IProcessQueries queries, Func<T, EmailVerificationPurpose>[] purposes)
            : base(() => Resources.Validation_EmailVerificationTicket_IsWrongPurpose)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            if (purposes == null) throw new ArgumentNullException("purposes");
            _queries = queries;
            _purposes = purposes;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var ticket = (string)context.PropertyValue;
            if (string.IsNullOrWhiteSpace(ticket)) return true;
            var purposes = _purposes.Select(x => x((T)context.Instance)).ToArray();
            var entity = _queries.Execute(new EmailVerificationBy(ticket)).Result;
            if (entity == null) return true;
            if (purposes.Contains(entity.Purpose)) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }
}
