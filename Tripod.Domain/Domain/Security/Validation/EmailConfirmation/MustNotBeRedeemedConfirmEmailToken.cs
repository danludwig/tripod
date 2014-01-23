﻿using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public class MustNotBeRedeemedConfirmEmailToken : PropertyValidator
    {
        private readonly IProcessQueries _queries;

        internal MustNotBeRedeemedConfirmEmailToken(IProcessQueries queries)
            : base(() => Resources.Validation_EmailConfirmationTicket_IsRedeemed)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            _queries = queries;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var token = (string)context.PropertyValue;
            var userToken = _queries.Execute(new EmailConfirmationUserToken(token)).Result;
            if (userToken == null) return true;
            var ticket = userToken.Value;

            if (string.IsNullOrWhiteSpace(ticket)) return true;
            var entity = _queries.Execute(new EmailConfirmationBy(ticket)).Result;
            if (entity == null) return true;
            if (!entity.RedeemedOnUtc.HasValue) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }

    public static class MustNotBeRedeemedConfirmEmailTokenExtensions
    {
        public static IRuleBuilderOptions<T, string> MustNotBeRedeemedConfirmEmailToken<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries)
        {
            return ruleBuilder.SetValidator(new MustNotBeRedeemedConfirmEmailToken(queries));
        }
    }
}