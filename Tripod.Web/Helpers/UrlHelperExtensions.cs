using System;
using System.Web;
using System.Web.Mvc;

namespace Tripod.Web
{
    public static class UrlHelperExtensions
    {
        public static string AbsoluteAction(this UrlHelper urlHelper, ActionResult action)
        {
            var requestUrl = urlHelper.RequestContext.HttpContext.Request.Url;
            if (requestUrl == null)
                throw new InvalidOperationException("HttpRequest.Url was unexpectedly null.");
            var actionUrl = urlHelper.Action(action);
            return string.Format("{0}://{1}{2}", requestUrl.Scheme, requestUrl.Authority, actionUrl);
        }

        internal static string AbsoluteActionFormat(this UrlHelper urlHelper, ActionResult action)
        {
            var requestUrl = urlHelper.RequestContext.HttpContext.Request.Url;
            if (requestUrl == null)
                throw new InvalidOperationException("HttpRequest.Url was unexpectedly null.");
            var encodedUrlFormat = urlHelper.Action(action);
            var decodedUrlFormat = HttpUtility.UrlDecode(encodedUrlFormat);
            var verifyUrlFormat = string.Format("{0}://{1}{2}",
                requestUrl.Scheme, requestUrl.Authority, decodedUrlFormat);
            return verifyUrlFormat;
        }
    }
}