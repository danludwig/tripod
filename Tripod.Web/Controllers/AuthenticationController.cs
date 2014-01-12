using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using FluentValidation.Mvc;
using Microsoft.AspNet.Identity;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class AuthenticationController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;
        private readonly IProcessValidation _validation;

        public AuthenticationController(IProcessQueries queries, IProcessCommands commands, IProcessValidation validation)
        {
            _queries = queries;
            _commands = commands;
            _validation = validation;
        }

        [AllowAnonymous]
        [HttpGet, Route("sign-in")]
        public virtual ActionResult SignIn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(Views.SignIn);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-in")]
        public virtual async Task<ActionResult> SignIn(SignIn command, string returnUrl)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (!ModelState.IsValid) return View(Views.SignIn, command);
            await _commands.Execute(command);
            return RedirectToLocal(returnUrl);
        }

        [AllowAnonymous]
        [HttpPost, Route("sign-in/validate/{fieldName?}")]
        public virtual ActionResult SignInValidate(SignIn command, string fieldName = null)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            var result = new ValidatedFields(ModelState, fieldName);
            return new CamelCaseJsonResult(result);
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
            var loginInfo = await _queries.Execute(new GetRemoteMembershipTicket(User));
            if (loginInfo == null) return RedirectToAction(MVC.Authentication.SignIn());

            //var externalIdentity = await HttpContext.GetOwinContext().Authentication
            //    .GetExternalIdentityAsync(DefaultAuthenticationTypes.ExternalCookie);
            //var displayName = externalIdentity.Name;
            //var email = externalIdentity.FindFirstValue(ClaimTypes.Email);

            // Sign in the user with this external login provider if the user already has a login
            var user = await _queries.Execute(new UserBy(loginInfo.Login));
            if (user != null)
            {
                await _commands.Execute(new SignOn
                {
                    LoginProvider = loginInfo.Login.LoginProvider,
                    ProviderKey = loginInfo.Login.ProviderKey,
                });
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

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/external-login/confirm")]
        public virtual async Task<ActionResult> ExternalLoginConfirmation(CreateRemoteMembership command, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction(await MVC.Account.Manage());

            // Get the information about the user from the external login provider
            var info = await _queries.Execute(new GetRemoteMembershipTicket(User));
            if (info == null) return RedirectToAction(MVC.Authentication.ExternalLoginFailure());

            if (ModelState.IsValid)
            {
                await _commands.Execute(command);
                await _commands.Execute(new SignOn
                {
                    LoginProvider = info.Login.LoginProvider,
                    ProviderKey = info.Login.ProviderKey,
                });
                return RedirectToLocal(returnUrl);
            }

            ViewBag.LoginProvider = info.Login.LoginProvider;
            ViewBag.ReturnUrl = returnUrl;
            return View(MVC.Account.Views.ExternalLoginConfirmation, command);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/link-login")]
        public virtual ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action(MVC.Authentication.LinkLoginCallback()), User.Identity.GetUserId());
        }

        [HttpGet, Route("account/link-login/complete")]
        public virtual async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await _queries.Execute(new GetRemoteMembershipTicket(User));
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

        [HttpPost, Route("account/logoff")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult LogOff()
        {
            _commands.Execute(new SignOut());
            return RedirectToAction(MVC.Home.Index());
        }

        [AllowAnonymous]
        [HttpGet, Route("account/external-login/failed")]
        public virtual ActionResult ExternalLoginFailure()
        {
            return View(MVC.Account.Views.ExternalLoginFailure);
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