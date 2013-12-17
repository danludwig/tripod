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
    public class AccountController : Controller
    {
        private readonly UserManager<User, int> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthenticate _authenticator;
        private readonly IProcessCommands _commands;
        private readonly IProcessValidation _validation;

        public AccountController(UserManager<User, int> userManager, IUnitOfWork unitOfWork, IAuthenticate authenticator, IProcessCommands commands, IProcessValidation validation)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _authenticator = authenticator;
            _commands = commands;
            _validation = validation;
        }

        //[AllowAnonymous]
        //[HttpGet, Route("account/login")]
        //public ActionResult Login(string returnUrl)
        //{
        //    ViewBag.ReturnUrl = returnUrl;
        //    return View();
        //}

        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //[HttpPost, Route("account/login")]
        //public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        //{
        //    if (!ModelState.IsValid) return View(model);
        //    var user = await _userManager.FindAsync(model.UserName, model.Password);
        //    if (user != null)
        //    {
        //        await SignInAsync(user, model.RememberMe);
        //        return RedirectToLocal(returnUrl);
        //    }
        //    // ReSharper disable LocalizableElement
        //    ModelState.AddModelError("", "Invalid username or password.");
        //    // ReSharper restore LocalizableElement

        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

        [AllowAnonymous]
        [HttpGet, Route("account/register")]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/register")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            var command = new CreateUser { Name = model.UserName };
            var validation = _validation.Validate(command);
            ModelState.AddModelErrors(validation);
            if (!ModelState.IsValid) return View(model);
            await _commands.Execute(command);
            //var user = new User { Name = model.UserName };
            var result = await _userManager.CreateAsync(command.Created, model.Password);
            await _unitOfWork.SaveChangesAsync();
            if (result.Succeeded)
            {
                await SignInAsync(command.Created, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }
            AddErrors(result);

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/disassociate")]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            var result = await _userManager.RemoveLoginAsync(int.Parse(User.Identity.GetUserId()), new UserLoginInfo(loginProvider, providerKey));
            var message = result.Succeeded ? ManageMessageId.RemoveLoginSuccess : ManageMessageId.Error;
            return RedirectToAction("Manage", new { Message = message });
        }

        [HttpGet, Route("account/manage")]
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/manage")]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            var hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (!ModelState.IsValid) return View(model);
                var result = await _userManager.ChangePasswordAsync(int.Parse(User.Identity.GetUserId()), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                }
                AddErrors(result);
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                var state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (!ModelState.IsValid) return View(model);
                var result = await _userManager.AddPasswordAsync(int.Parse(User.Identity.GetUserId()), model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/external-login")]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        [AllowAnonymous]
        [HttpGet, Route("account/external-login/received")]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await _userManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            // If the user does not have an account, then prompt the user to create an account
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
            return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/link-login")]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        [HttpGet, Route("account/link-login/complete")]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await _userManager.AddLoginAsync(int.Parse(User.Identity.GetUserId()), loginInfo.Login);
            return result.Succeeded
                ? RedirectToAction("Manage")
                : RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/external-login/confirm")]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var createUser = new CreateUser { Name = model.UserName };
                await _commands.Execute(createUser);
                //var user = new User { Name = model.UserName };
                var result = await _userManager.CreateAsync(createUser.Created);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(createUser.Created.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(createUser.Created, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost, Route("account/logoff")]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //AuthenticationManager.SignOut();
            _authenticator.SignOff();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        [HttpGet, Route("account/external-login/failed")]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        [HttpGet, Route("account/external-logins")]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = _userManager.GetLogins(int.Parse(User.Identity.GetUserId()));
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return PartialView("_RemoveAccountPartial", linkedAccounts);
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
        private const string XsrfKey = "XsrfId";

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
            return RedirectToAction("Index", "Home");
        }

        private class ChallengeResult : HttpUnauthorizedResult
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
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}