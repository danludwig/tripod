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
        public virtual ActionResult GetForm()
        {
            var hasPassword = _queries.Execute(new UserHasLocalMembership(User)).Result;

            if (ControllerContext.IsChildAction)
                return PartialView(MVC.LocalMemberships.Views.AddPasswordForm);

            ViewBag.AsPartial = false;
            return View(MVC.LocalMemberships.Views.AddPasswordForm);
        }

        [Authorize]
        [HttpPost, Route("account/password")]
        public async virtual Task<ActionResult> Post(CreateLocalMembership command)
        {
            if (ModelState.IsValid)
            {
                await _commands.Execute(command);
                return RedirectToAction(MVC.Account.Manage(AccountController.ManageMessageId.SetPasswordSuccess));
            }

            ViewBag.AsPartial = false;
            return View(MVC.LocalMemberships.Views.AddPasswordForm, command);
        }
    }
}