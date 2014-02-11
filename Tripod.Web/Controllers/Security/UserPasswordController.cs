using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    [Authorize]
    public partial class UserPasswordController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        public UserPasswordController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("settings/password")]
        public virtual async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetAppUserId();
            var user = await _queries.Execute(new UserViewBy(userId));
            var localMembership = await _queries.Execute(new LocalMembershipByUser(userId));

            var model = new CreateLocalMembershipModel
            {
                UserView = user,
                Command = new CreateLocalMembership(),
            };

            return View(MVC.Security.Views.CreatePassword, model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("settings/password")]
        public virtual async Task<ActionResult> CreateLocalMembership(CreateLocalMembership command)
        {
            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                var userId = User.Identity.GetAppUserId();
                var user = await _queries.Execute(new UserViewBy(userId));
                var model = new CreateLocalMembershipModel
                {
                    UserView = user,
                    Command = command,
                };
                return View(MVC.Security.Views.CreatePassword, model);
            }

            return RedirectToAction(await MVC.UserPassword.Index());
        }
    }
}