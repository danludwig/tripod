using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustFindEmailConfirmationByTicket : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustFindEmailConfirmationByTicket(IProcessQueries queries)
            : base(() => Resources.Validation_DoesNotExist)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var ticket = (string)context.PropertyValue;
            if (string.IsNullOrWhiteSpace(ticket)) return false;

            var entity = _queries.Execute(new EmailConfirmationBy(ticket)).Result;
            return entity != null;
        }
    }

    public static class MustFindEmailConfirmationByTicketExtensions
    {
        public static IRuleBuilderOptions<T, string> MustFindEmailConfirmationByTicket<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustFindEmailConfirmationByTicket(queries));
        }
    }
}
