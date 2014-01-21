using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotBeRedeemedConfirmationTicket : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustNotBeRedeemedConfirmationTicket(IProcessQueries queries)
            : base(() => Resources.Validation_EmailConfirmationTicket_IsRedeemed)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var ticket = (string)context.PropertyValue;
            if (string.IsNullOrWhiteSpace(ticket)) return true;
            var entity = _queries.Execute(new EmailConfirmationBy(ticket)).Result;
            if (entity == null) return true;
            return !entity.RedeemedOnUtc.HasValue;
        }
    }

    public static class MustNotBeRedeemedConfirmationTicketExtensions
    {
        public static IRuleBuilderOptions<T, string> MustNotBeRedeemedConfirmationTicket<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBeRedeemedConfirmationTicket(queries));
        }
    }
}
