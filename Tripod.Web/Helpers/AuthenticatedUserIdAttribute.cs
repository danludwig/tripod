using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace Tripod.Web
{
    public class AuthenticatedUserIdAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAuthenticated)
            {
                var userIdString = filterContext.HttpContext.User.Identity.GetUserId();
                int userIdInt;
                if (int.TryParse(userIdString, out userIdInt))
                {
                    if (userIdInt > 0)
                    {
                        filterContext.Controller.ViewBag.AuthenticatedUserId = userIdInt;
                    }
                }
            }
        }
    }
}