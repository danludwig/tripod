using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class AuthenticationController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        public AuthenticationController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [AllowAnonymous]
        [HttpGet, Route("account/login")]
        public virtual ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(MVC.Account.Views.Login);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/login")]
        public virtual async Task<ActionResult> Login(SignIn command, string returnUrl)
        {
            if (!ModelState.IsValid) return View(MVC.Account.Views.Login, command);
            await _commands.Execute(command);
            return RedirectToLocal(returnUrl);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/external-login")]
        public virtual ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action(MVC.Authentication.ExternalLoginCallback(returnUrl)));
        }

        [AllowAnonymous]
        [HttpGet, Route("account/external-login/received")]
        public virtual async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await _queries.Execute(new GetRemoteMembershipTicket());
            if (loginInfo == null) return RedirectToAction(MVC.Authentication.Login());

            // Sign in the user with this external login provider if the user already has a login
            var user = await _queries.Execute(new UserBy(loginInfo.Login));
            if (user != null)
            {
                await _commands.Execute(new SignOn { UserLoginInfo = loginInfo.Login });
                return RedirectToLocal(returnUrl);
            }
            // If the user does not have an account, then prompt the user to create an account
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
            var model = new CreateRemoteMembership
            {
                UserName = loginInfo.UserName
            };
            return View(MVC.Account.Views.ExternalLoginConfirmation, model);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(MVC.Home.Index());
        }
    }
}