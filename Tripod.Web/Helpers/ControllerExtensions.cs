using System.Web.Mvc;
using FluentValidation.Results;

namespace Tripod.Web
{
    public static class ControllerExtensions
    {
        [UsedImplicitly]
        public static void AddModelErrors(this ModelStateDictionary modelState, ValidationResult validationResult)
        {
            if (modelState == null || validationResult == null || validationResult.IsValid) return;
            foreach (var error in validationResult.Errors)
                modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        public static ActionResult RedirectToLocal(this Controller controller, string url, ActionResult fallback = null)
        {
            if (controller.Url.IsLocalUrl(url))
                return new RedirectResult(url);

            if (fallback != null && controller.Url.IsLocalUrl(controller.Url.Action(fallback)))
                return new RedirectToRouteResult(fallback.GetT4MVCResult().RouteValueDictionary);

            return new RedirectToRouteResult(MVC.Home.Index().GetT4MVCResult().RouteValueDictionary);
        }
    }
}