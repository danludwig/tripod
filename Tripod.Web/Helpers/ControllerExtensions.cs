using System.Web.Mvc;
using FluentValidation.Results;

namespace Tripod.Web
{
    public static class ControllerExtensions
    {
        public static void AddModelErrors(this ModelStateDictionary modelState, ValidationResult validationResult)
        {
            if (modelState == null || validationResult == null || validationResult.IsValid) return;
            foreach (var error in validationResult.Errors)
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}