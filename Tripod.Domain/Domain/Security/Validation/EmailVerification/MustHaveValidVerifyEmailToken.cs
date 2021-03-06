﻿using System;
using FluentValidation;
using FluentValidation.Validators;

namespace Tripod.Domain.Security
{
    public static class MustHaveValidVerifyEmailTokenExtensions
    {
        /// <summary>
        /// Validates that this EmailVerification Ticket has a valid Token.
        /// </summary>
        /// <typeparam name="T">The command with the EmailVerification Ticket to validate.</typeparam>
        /// <param name="ruleBuilder">Fluent rule builder options.</param>
        /// <param name="queries">Query processor instance, for locating EmailVerification by Ticket.</param>
        /// <param name="token">The Token to validate this EmailVerification Ticket against.</param>
        /// <returns>Fluent rule builder options.</returns>
        public static IRuleBuilderOptions<T, string> MustHaveValidVerifyEmailToken<T>
            (this IRuleBuilder<T, string> ruleBuilder, IProcessQueries queries, Func<T, string> token)
        {
            return ruleBuilder.SetValidator(new MustHaveValidVerifyEmailToken<T>(queries, token));
        }
    }

    internal class MustHaveValidVerifyEmailToken<T> : PropertyValidator
    {
        private readonly IProcessQueries _queries;
        private readonly Func<T, string> _token;

        internal MustHaveValidVerifyEmailToken(IProcessQueries queries, Func<T, string> token)
            : base(() => Resources.Validation_EmailVerificationTicket_HasInvalidToken)
        {
            if (queries == null) throw new ArgumentNullException("queries");
            if (token == null) throw new ArgumentNullException("token");
            _queries = queries;
            _token = token;
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            var ticket = (string)context.PropertyValue;
            var token = _token((T)context.Instance);

            // there should be another validator which detects emptiness of ticket
            if (string.IsNullOrWhiteSpace(ticket)) return true;

            var verification = _queries.Execute(new EmailVerificationBy(ticket)).Result;

            // there should be another validator which detects the presence of the entity by ticket
            if (verification == null) return true;

            // token is not valid unless it matches the ticket's token
            var query = new EmailVerificationTokenIsValid(token, ticket, verification.Purpose);
            var isValid = _queries.Execute(query).Result && verification.Token == token
                && !string.IsNullOrWhiteSpace(token);
            if (isValid) return true;

            context.MessageFormatter.AppendArgument("PropertyName", context.PropertyDescription.ToLower());
            return false;
        }
    }
}
