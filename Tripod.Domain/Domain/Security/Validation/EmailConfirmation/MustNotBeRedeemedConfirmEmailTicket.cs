using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotBeRedeemedConfirmEmailTicket : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustNotBeRedeemedConfirmEmailTicket(IProcessQueries queries)
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
            if (!entity.RedeemedOnUtc.HasValue) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }

    public static class MustNotBeRedeemedConfirmEmailTicketExtensions
    {
        public static IRuleBuilderOptions<T, string> MustNotBeRedeemedConfirmEmailTicket<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBeRedeemedConfirmEmailTicket(queries));
        }
    }
}
