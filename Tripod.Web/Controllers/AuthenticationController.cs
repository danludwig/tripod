using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class AuthenticationController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;
        private readonly IProcessValidation _validation;

        [UsedImplicitly]
        public AuthenticationController(IProcessQueries queries, IProcessCommands commands, IProcessValidation validation)
        {
            _queries = queries;
            _commands = commands;
            _validation = validation;
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/external-login/confirm")]
        public virtual async Task<ActionResult> ExternalLoginConfirmation(CreateRemoteMembership command, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction(await MVC.Account.Manage());

            // todo: make sure there is a confirmed email address..?

            // Get the information about the user from the external login provider
            var info = await _queries.Execute(new PrincipalRemoteMembershipTicket(User));
            if (info == null) return RedirectToAction(MVC.Authentication.ExternalLoginFailure());

            if (ModelState.IsValid)
            {
                await _commands.Execute(command);
                await _commands.Execute(new SignOn
                {
                    LoginProvider = info.Login.LoginProvider,
                    ProviderKey = info.Login.ProviderKey,
                });
                return this.RedirectToLocal(returnUrl);
            }

            ViewBag.LoginProvider = info.Login.LoginProvider;
            ViewBag.ReturnUrl = returnUrl;
            return View(MVC.Account.Views.ExternalLoginConfirmation, command);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/link-login")]
        public virtual ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action(MVC.Authentication.LinkLoginCallback()), User.Identity.GetUserId());
        }

        [Authorize]
        [HttpGet, Route("account/link-login/complete")]
        public virtual async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await _queries.Execute(new PrincipalRemoteMembershipTicket(User));
            if (loginInfo == null) return RedirectToAction(await MVC.Account.Manage(AccountController.ManageMessageId.Error));

            var command = new CreateRemoteMembership
            {
                Principal = User,
            };
            var validation = _validation.Validate(command);
            if (!validation.IsValid) return RedirectToAction(await MVC.Account.Manage(AccountController.ManageMessageId.Error));

            await _commands.Execute(command);
            return RedirectToAction(await MVC.Account.Manage());
        }

        [Authorize]
        [HttpPost, Route("account/logoff")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult LogOff()
        {
            _commands.Execute(new SignOut());
            return RedirectToAction(MVC.Home.Index());
        }

        [HttpGet, Route("account/external-login/failed")]
        public virtual ActionResult ExternalLoginFailure()
        {
            return View(MVC.Account.Views.ExternalLoginFailure);
        }

        //private ActionResult RedirectToLocal(string returnUrl)
        //{
        //    if (Url.IsLocalUrl(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    return RedirectToAction(MVC.Home.Index());
        //}
    }
}