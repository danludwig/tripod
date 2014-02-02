using System.Web.Mvc;

namespace Tripod.Web
{
    public static class ControllerExtensions
    {
        public static ActionResult RedirectToLocal(this Controller controller, string url, ActionResult fallback = null)
        {
            return controller.Url.IsLocalUrl(url) ? new RedirectResult(url) : controller.RedirectToLocal(fallback);
        }

        public static ActionResult RedirectToLocal(this Controller controller, ActionResult action)
        {
            if (action != null && controller.Url.IsLocalUrl(controller.Url.Action(action)))
                return new RedirectToRouteResult(action.GetT4MVCResult().RouteValueDictionary);

            return new RedirectToRouteResult(MVC.Home.Index().GetT4MVCResult().RouteValueDictionary);
        }
    }
}