using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Owin;
using Claim = System.Security.Claims.Claim;

namespace Tripod.Web
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        [UsedImplicitly]
        public static void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/sign-in"),
            });

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            app.UseTwitterAuthentication(
               consumerKey: "ROkPQ6Lcu3TZ3NEEgAiw",
               consumerSecret: "XbuHawYulw4K385lrstlWlyn8j7Masp1Bvt2i3k");

            var facebookAuthenticationOptions = new FacebookAuthenticationOptions();
            facebookAuthenticationOptions.Scope.Add("email");
            facebookAuthenticationOptions.AppId = "406322042831063";
            facebookAuthenticationOptions.AppSecret = "682d4c4acb90e7cfecc66635256560d4";
            facebookAuthenticationOptions.Provider = new FacebookAuthenticationProvider
            {
                OnAuthenticated = x =>
                {
                    x.Identity.AddClaim(new Claim("FacebookAccessToken", x.AccessToken));
                    return Task.FromResult(0);
                }
            };
            facebookAuthenticationOptions.SignInAsAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;
            app.UseFacebookAuthentication(facebookAuthenticationOptions);

            //app.UseFacebookAuthentication(
            //   appId: "406322042831063",
            //   appSecret: "682d4c4acb90e7cfecc66635256560d4");

            app.UseGoogleAuthentication();
        }
    }
}