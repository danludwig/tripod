using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace Tripod.Web
{
    public static class UrlHelperExtensions
    {
        public static string AbsoluteAction(this UrlHelper urlHelper, Uri requestUrl, ActionResult action)
        {
            Debug.Assert(requestUrl != null);
            var actionUrl = urlHelper.Action(action);
            return string.Format("{0}://{1}{2}", requestUrl.Scheme, requestUrl.Authority, actionUrl);
        }
    }
}