using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class LocalMembershipsController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        public LocalMembershipsController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [Authorize]
        [HttpGet, Route("account/password")]
        public virtual ActionResult Form()
        {
            var hasPassword = _queries.Execute(new UserHasLocalMembership(User)).Result;
            var view = hasPassword
                ? MVC.LocalMemberships.Views.ChangePasswordForm
                : MVC.LocalMemberships.Views.AddPasswordForm;

            if (ControllerContext.IsChildAction) return PartialView(view);

            ViewBag.AsPartial = false;
            return View(view);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/password")]
        public async virtual Task<ActionResult> CreatePassword(CreateLocalMembership command)
        {
            if (ModelState.IsValid)
            {
                await _commands.Execute(command);
                return RedirectToAction(MVC.Account.Manage(AccountController.ManageMessageId.SetPasswordSuccess));
            }

            ViewBag.AsPartial = false;
            return View(MVC.LocalMemberships.Views.AddPasswordForm, command);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/password/change")]
        public async virtual Task<ActionResult> ChangePassword(ChangeLocalPassword command)
        {
            if (ModelState.IsValid)
            {
                await _commands.Execute(command);
                return RedirectToAction(MVC.Account.Manage(AccountController.ManageMessageId.ChangePasswordSuccess));
            }

            ViewBag.AsPartial = false;
            return View(MVC.LocalMemberships.Views.ChangePasswordForm, command);
        }
    }
}