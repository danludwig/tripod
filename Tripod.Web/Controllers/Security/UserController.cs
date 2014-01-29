using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class UserController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        public UserController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("users/{userId:int}")]
        public virtual async Task<ActionResult> ById(int userId)
        {
            var view = await _queries.Execute(new UserViewBy(userId));
            if (view == null) return HttpNotFound();

            var model = new ChangeUserNameModel
            {
                UserView = view,
                Command = new ChangeUserName
                {
                    UserId = userId,
                    UserName = User.Identity.Name,
                },
            };

            return View(MVC.Security.Views.User, model);
        }

        [ValidateAntiForgeryToken]
        [HttpPut, Route("users/{userId:int}/name")]
        public virtual async Task<ActionResult> ChangeName(ChangeUserName command)
        {
            if (command == null) return View(MVC.Errors.BadRequest());

            var view = await _queries.Execute(new UserViewBy(command.UserId));
            if (view == null) return HttpNotFound();

            if (!ModelState.IsValid)
            {
                var model = new ChangeUserNameModel
                {
                    UserView = view,
                    Command = command,
                };

                return View(MVC.Security.Views.User, model);
            }

            await _commands.Execute(command);
            Response.ClientCookie(command.SignedIn.Id, _queries);
            return RedirectToAction(await MVC.User.ById(command.UserId));
        }

        [HttpPost, Route("users/{userId:int}/name/validate")]
        public virtual ActionResult ValidateChangeName(ChangeUserName command)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));
            if (command == null)
            {
                Response.StatusCode = 400;
                return Json(null);
            }

            var result = new ValidatedFields(ModelState);

            //ModelState[command.PropertyName(x => x.UserName)].Errors.Clear();
            //result = new ValidatedFields(ModelState, command.PropertyName(x => x.UserName));

            return new CamelCaseJsonResult(result);
        }
    }
}