using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using FluentValidation.Results;

namespace Tripod.Web.Models
{
    public class ValidatedFields : Dictionary<string, ValidatedField>
    {
        public ValidatedFields([UsedImplicitly] ModelStateDictionary modelState, params string[] fieldNames)
        {
            if (modelState == null) return;
            var fieldState = fieldNames.Length > 0
                ? modelState.Where(x => fieldNames.Any(y => x.Key.Equals(y, StringComparison.OrdinalIgnoreCase))).ToArray()
                : modelState.ToArray();
            if (!fieldState.Any()) return;

            foreach (var entry in fieldState)
            {
                var name = entry.Key;
                var state = entry.Value;
                var validatedField = new ValidatedField();
                if (state.Value != null)
                {
                    validatedField.AttemptedValue = state.Value.AttemptedValue;
                }
                if (state.Errors != null)
                {
                    validatedField.Errors = state.Errors
                        .Select(x => new ValidatedFieldError
                        {
                            Message = x.ErrorMessage,
                            CustomState = x.Exception == null ? null : x.Exception.GetType().Name,
                        })
                        .ToArray();
                }
                Add(name, validatedField);
            }
        }

        [UsedImplicitly]
        public ValidatedFields(ValidationResult validationResult, params string[] fieldNames)
        {
            if (validationResult == null) return;
            var validationFailures = fieldNames.Length > 0
                ? validationResult.Errors.Where(x => fieldNames.Any(y => x.PropertyName.Equals(y, StringComparison.OrdinalIgnoreCase))).ToArray()
                : validationResult.Errors.ToArray();
            if (!validationFailures.Any()) return;

            foreach (var validationFailure in validationFailures)
            {
                var name = validationFailure.PropertyName;
                
                // may already have a validated field for this
                var validatedField = ContainsKey(name) ? this[name] : null;
                if (validatedField == null)
                {
                    validatedField = new ValidatedField
                    {
                        AttemptedValue = validationFailure.AttemptedValue,
                        Errors = new List<ValidatedFieldError>(),
                    };
                    this[name] = validatedField;
                }
                validatedField.Errors.Add(new ValidatedFieldError
                {
                    Message = validationFailure.ErrorMessage,
                    CustomState = validationFailure.CustomState,
                });
            }
        }
}
}