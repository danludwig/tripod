using System.Web.Mvc;

namespace Tripod.Web
{
    public static class ControllerExtensions
    {
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