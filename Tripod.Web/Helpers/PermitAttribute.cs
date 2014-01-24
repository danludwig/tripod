using System.Web.Mvc;

namespace Tripod.Web
{
    public class PermitAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!string.IsNullOrWhiteSpace(filterContext.HttpContext.User.Identity.Name))
            {
                filterContext.HttpContext.Response.StatusCode = 403;
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }
}