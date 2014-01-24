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

        public static ActionResult RedirectToLocal(this Controller controller, string url)
        {
            if (controller.Url.IsLocalUrl(url))
                return new RedirectResult(url);

            var localAction = MVC.Home.Index();
            var callInfo = localAction.GetT4MVCResult();
            return new RedirectToRouteResult(callInfo.RouteValueDictionary);
        }
    }
}