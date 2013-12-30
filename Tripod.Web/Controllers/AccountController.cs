using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    [Authorize]
    public partial class AccountController : Controller
    {
        private readonly UserManager<User, int> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticate _authenticator;
        private readonly IProcessCommands _commands;
        //private readonly IProcessValidation _validation;
        private readonly IProcessQueries _queries;

        public AccountController(IProcessQueries queries
            , IProcessCommands commands
            //, IProcessValidation validation
            , IUnitOfWork unitOfWork
            , IAuthenticate authenticator
            , UserManager<User, int> userManager
        )
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _authenticator = authenticator;
            _commands = commands;
            _queries = queries;
            //_validation = validation;
        }

        //[ValidateAntiForgeryToken]
        //[HttpPost, Route("account/disassociate")]
        //public virtual async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        //{
        //    var result = await _userManager.RemoveLoginAsync(int.Parse(User.Identity.GetUserId()), new UserLoginInfo(loginProvider, providerKey));
        //    var message = result.Succeeded ? ManageMessageId.RemoveLoginSuccess : ManageMessageId.Error;
        //    return RedirectToAction(MVC.Account.Manage(message));
        //}

        [HttpGet, Route("account/manage")]
        public virtual ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action(MVC.Account.Manage());
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/link-login")]
        public virtual ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action(MVC.Account.LinkLoginCallback()), User.Identity.GetUserId());
        }

        [HttpGet, Route("account/link-login/complete")]
        public virtual async Task<ActionResult> LinkLoginCallback()
        {
            //var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            var loginInfo = await _queries.Execute(new GetRemoteMembershipTicket(User, ChallengeResult.XsrfKey));
            if (loginInfo == null)
            {
                return RedirectToAction(MVC.Account.Manage(ManageMessageId.Error));
            }
            var result = await _userManager.AddLoginAsync(int.Parse(User.Identity.GetUserId()), loginInfo.Login);
            await _unitOfWork.SaveChangesAsync();
            return result.Succeeded
                ? RedirectToAction(MVC.Account.Manage())
                : RedirectToAction(MVC.Account.Manage(ManageMessageId.Error));
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/external-login/confirm")]
        public virtual async Task<ActionResult> ExternalLoginConfirmation(CreateRemoteMembership command, string returnUrl)
        {
            if (User.Identity.IsAuthenticated) return RedirectToAction(MVC.Account.Manage());

            // Get the information about the user from the external login provider
            //var info = await AuthenticationManager.GetExternalLoginInfoAsync();
            var info = await _queries.Execute(new GetRemoteMembershipTicket());
            if (info == null) return View(MVC.Account.Views.ExternalLoginFailure);

            if (ModelState.IsValid)
            {
                //var command = new CreateRemoteMembership
                //{
                //    Principal = User,
                //    UserName = model.UserName,
                //};
                await _commands.Execute(command);
                await _commands.Execute(new SignOn
                {
                    UserLoginInfo = info.Login,
                });
                //var createUser = new CreateUser { Name = model.UserName };
                //await _commands.Execute(createUser);
                //var result = await _userManager.CreateAsync(createUser.Created);
                //await _unitOfWork.SaveChangesAsync();
                //if (result.Succeeded)
                //{
                //    result = await _userManager.AddLoginAsync(createUser.Created.Id, info.Login);
                //    await _unitOfWork.SaveChangesAsync();
                //    if (result.Succeeded)
                //    {
                //        await SignInAsync(createUser.Created, isPersistent: false);
                return RedirectToLocal(returnUrl);
                //    }
                //}
                //AddErrors(result);
            }

            ViewBag.LoginProvider = info.Login.LoginProvider;
            ViewBag.ReturnUrl = returnUrl;
            return View(command);
        }

        [HttpPost, Route("account/logoff")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult LogOff()
        {
            //AuthenticationManager.SignOut();
            _authenticator.SignOff();
            return RedirectToAction(MVC.Home.Index());
        }

        [AllowAnonymous]
        [HttpGet, Route("account/external-login/failed")]
        public virtual ActionResult ExternalLoginFailure()
        {
            return View();
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && _userManager != null)
        //    {
        //        _userManager.Dispose();
        //        //UserManager = null;
        //    }
        //    base.Dispose(disposing);
        //}

        #region Helpers

        // Used for XSRF protection when adding external logins
        //private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(User user, bool isPersistent)
        {
            await _authenticator.SignOff();
            await _authenticator.SignOn(user, isPersistent);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = _userManager.FindById(int.Parse(User.Identity.GetUserId()));
            if (user != null && user.LocalMembership != null)
            {
                return user.LocalMembership.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(MVC.Home.Index());
        }
        #endregion
    }
}