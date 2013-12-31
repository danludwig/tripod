using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;
using Tripod.Ioc.Configuration;

namespace Tripod.Web
{
    public class ChallengeResult : HttpUnauthorizedResult
    {
        public ChallengeResult(string provider, string redirectUri, string userId = null)
        {
            LoginProvider = provider;
            RedirectUri = redirectUri;
            UserId = userId;
        }

        private string LoginProvider { get; set; }
        private string RedirectUri { get; set; }
        private string UserId { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
            if (UserId != null)
            {
                properties.Dictionary[ConfigurationManager.AppSettings[AppSettingKey.XsrfKey.ToString()]] = UserId;
            }
            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        }
    }
}