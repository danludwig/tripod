using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace Tripod.Web
{
    public class ChallengeResult : HttpUnauthorizedResult
    {
        private readonly AppConfiguration _appConfiguration;
        private readonly string _loginProvider;
        private readonly string _redirectUri;
        private readonly string _userId;

        public ChallengeResult(AppConfiguration appConfiguration,
            string provider, string redirectUri, string userId = null)
        {
            _appConfiguration = appConfiguration;
            _loginProvider = provider;
            _redirectUri = redirectUri;
            _userId = userId;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new AuthenticationProperties { RedirectUri = _redirectUri };
            if (_userId != null)
            {
                properties.Dictionary[_appConfiguration.XsrfKey] = _userId;
            }
            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, _loginProvider);
        }
    }
}