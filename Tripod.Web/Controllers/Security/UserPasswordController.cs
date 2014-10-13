using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    [Authorize]
    public partial class UserPasswordController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public UserPasswordController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("settings/password")]
        public virtual async Task<ActionResult> Index()
        {
            var userId = User.Identity.GetUserId<int>();
            var user = await _queries.Execute(new UserViewBy(userId));
            var localMembership = await _queries.Execute(new LocalMembershipByUser(userId));
            if (localMembership != null)
            {
                var model = new ChangeLocalPasswordModel
                {
                    UserView = user,
                    Command = new ChangeLocalPassword(),
                };
                return View(MVC.Security.Views.ChangePassword, model);
            }
            else
            {
                var model = new CreateLocalMembershipModel
                {
                    UserView = user,
                    Command = new CreateLocalMembership(),
                };
                return View(MVC.Security.Views.CreatePassword, model);
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("settings/password")]
        public virtual async Task<ActionResult> CreateLocalMembership(CreateLocalMembership command)
        {
            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId<int>();
                var user = await _queries.Execute(new UserViewBy(userId));
                var model = new CreateLocalMembershipModel
                {
                    UserView = user,
                    Command = command,
                };
                return View(MVC.Security.Views.CreatePassword, model);
            }

            await _commands.Execute(command);
            TempData.Alerts("**Your password was created successfully.**", AlertFlavor.Success, true);
            return RedirectToAction(await MVC.UserPassword.Index());
        }

        [ValidateAntiForgeryToken]
        [HttpPut, Route("settings/password")]
        public virtual async Task<ActionResult> ChangeLocalPassword(ChangeLocalPassword command)
        {
            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId<int>();
                var user = await _queries.Execute(new UserViewBy(userId));
                var model = new ChangeLocalPasswordModel
                {
                    UserView = user,
                    Command = command,
                };
                return View(MVC.Security.Views.ChangePassword, model);
            }

            await _commands.Execute(command);
            TempData.Alerts("**Your password was updated successfully.**", AlertFlavor.Success, true);
            return RedirectToAction(await MVC.UserPassword.Index());
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("settings/password/validate/{fieldName?}", Order = 1)]
        public virtual ActionResult ValidateChangeLocalPassword(ChangeLocalPassword command, string fieldName = null)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null)
            {
                Response.StatusCode = 400;
                return Json(null);
            }

            var result = new ValidatedFields(ModelState, fieldName);

            //ModelState[command.PropertyName(x => x.OldPassword)].Errors.Clear();
            //result = new ValidatedFields(ModelState, fieldName);

            return new CamelCaseJsonResult(result);
        }
    }
}