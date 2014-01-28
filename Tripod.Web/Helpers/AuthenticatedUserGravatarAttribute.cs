using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Tripod.Domain.Security;

namespace Tripod.Web
{
    public class AuthenticatedUserGravatarAttribute : ActionFilterAttribute
    {
        // todo: should probably put the gravatar hash in a cookie to avoid hitting storage on every request
        public IProcessQueries Queries { get; set; }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAuthenticated || filterContext.IsChildAction ||
                filterContext.HttpContext.Request.IsAjaxRequest()) return;

            var userIdString = filterContext.HttpContext.User.Identity.GetUserId();
            int userIdInt;
            if (!int.TryParse(userIdString, out userIdInt)) return;

            if (userIdInt <= 0) return;

            var hash = Queries.Execute(new HashedEmailValueBy(userIdInt)).Result;
            filterContext.Controller.ViewBag.AuthenticatedUserGravatar = hash;
        }
    }
}