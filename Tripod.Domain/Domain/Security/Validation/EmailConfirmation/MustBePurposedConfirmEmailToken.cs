using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustBePurposedConfirmEmailToken<T> : PropertyValidator
    {
        private readonly IProcessQueries _queries;
        private readonly Func<T, EmailConfirmationPurpose>[] _purposes;

        internal MustBePurposedConfirmEmailToken(IProcessQueries queries, params Func<T, EmailConfirmationPurpose>[] purposes)
            : base(() => Resources.Validation_EmailConfirmationTicket_IsWrongPurpose)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            if (purposes == null) throw new ArgumentNullException("purposes");
            _queries = queries;
            _purposes = purposes;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var token = (string)context.PropertyValue;
            var userToken = _queries.Execute(new EmailConfirmationUserToken(token)).Result;
            if (userToken == null) return true;
            var ticket = userToken.Value;

            var purposes = _purposes.Select(x => x((T)context.Instance)).ToArray();
            if (string.IsNullOrWhiteSpace(ticket)) return true;
            var entity = _queries.Execute(new EmailConfirmationBy(ticket)).Result;
            if (entity == null) return true;
            if (purposes.Contains(entity.Purpose)) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }

    public static class MustBePurposedConfirmEmailTokenExtensions
    {
        public static IRuleBuilderOptions<T, string> MustBePurposedConfirmEmailToken<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries, params Func<T, EmailConfirmationPurpose>[] purposes)
        {
            return ruleBuilder.SetValidator(new MustBePurposedConfirmEmailToken<T>(queries, purposes));
        }
    }
}
