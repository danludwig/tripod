using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    [Authorize]
    public partial class UserLoginsController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessValidation _validation;
        private readonly IProcessCommands _commands;
        private readonly AppConfiguration _appConfiguration;

        [UsedImplicitly]
        public UserLoginsController(
              IProcessQueries queries
            , IProcessValidation validation
            , IProcessCommands commands
            , AppConfiguration appConfiguration)
        {
            _queries = queries;
            _validation = validation;
            _commands = commands;
            _appConfiguration = appConfiguration;
        }

        [HttpGet, Route("settings/logins")]
        public virtual async Task<ActionResult> Index()
        {
            var user = await _queries.Execute(new UserViewBy(User.Identity.GetUserId<int>()));
            var logins = await _queries.Execute(new RemoteMembershipViewsBy(User.Identity.GetUserId<int>()));
            // allow deletion of social logins only when there is more than one or user has local password
            var isDeleteAllowed = logins.Count() > 1 || await _queries.Execute(new UserHasLocalMembership(User));

            var model = new LoginSettingsModel
            {
                UserView = user,
                Logins = logins.ToArray(),
                IsDeleteAllowed =  isDeleteAllowed,
            };

            ViewBag.ReturnUrl = Url.Action(MVC.UserLogins.Index());
            return View(MVC.Security.Views.User.Logins, model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("settings/logins")]
        public virtual ActionResult Post(string provider, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(provider)) return View(MVC.Errors.Views.BadRequest);

            // Request a redirect to the external login provider
            string redirectUri = Url.Action(MVC.UserLogins.LinkLoginCallback(provider, returnUrl));
            string userId = User.Identity.GetUserId();
            return new ChallengeResult(_appConfiguration, provider, redirectUri, userId);
        }

        [HttpGet, Route("settings/logins/callback")]
        public virtual async Task<ActionResult> LinkLoginCallback(string provider, string returnUrl)
        {
            string alert = null;
            var loginInfo = await _queries.Execute(new PrincipalRemoteMembershipTicket(User));
            if (loginInfo == null)
            {
                alert = string.Format("There was an error adding your **{0}** login, please try again.",
                    provider);
            }
            var command = new CreateRemoteMembership { Principal = User };
            var validationResult = _validation.Validate(command);
            if (!validationResult.IsValid)
            {
                alert = string.Format("There was an error adding your **{0}** login: {1}",
                    provider, validationResult.Errors
                        .First(x => !string.IsNullOrWhiteSpace(x.ErrorMessage)).ErrorMessage);
            }
            if (!string.IsNullOrWhiteSpace(alert))
            {
                TempData.Alerts(alert, AlertFlavor.Danger, true);
                return RedirectToAction(await MVC.UserLogins.Index());
            }

            await _commands.Execute(command);
            alert = string.Format("Your **{0}** login was added successfully.", provider);
            TempData.Alerts(alert, AlertFlavor.Success, true);
            return RedirectToAction(await MVC.UserLogins.Index());
        }

        [ValidateAntiForgeryToken]
        [HttpDelete, Route("settings/logins/{loginProvider}")]
        public virtual async Task<ActionResult> Delete(string loginProvider, DeleteRemoteMembership command)
        {
            if (command == null) return View(MVC.Errors.Views.BadRequest);

            string alert;
            if (!ModelState.IsValid)
            {
                var validationResult = _validation.Validate(command);
                alert = string.Format("There was an error removing your **{0}** login: {1}",
                    command.LoginProvider, validationResult.Errors
                        .First(x => !string.IsNullOrWhiteSpace(x.ErrorMessage)).ErrorMessage);
                TempData.Alerts(alert, AlertFlavor.Danger);
                return RedirectToAction(await MVC.UserLogins.Index());
            }

            await _commands.Execute(command);
            alert = string.Format("Your **{0}** login was removed successfully.", command.LoginProvider);
            TempData.Alerts(alert, AlertFlavor.Success, true);
            return RedirectToAction(await MVC.UserLogins.Index());
        }
    }
}