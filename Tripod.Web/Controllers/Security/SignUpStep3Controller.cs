using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class SignUpStep3Controller : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        public SignUpStep3Controller(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("sign-up/password")]
        public virtual async Task<ActionResult> Index(string token)
        {
            var userToken = await _queries.Execute(new EmailConfirmationUserToken(token));
            if (userToken == null) return HttpNotFound();
            var confirmation = await _queries.Execute(new EmailConfirmationBy(userToken.Value));
            if (confirmation == null) return HttpNotFound();

            // todo: confirmation cannot be expired, redeemed, or for different purpose

            ViewBag.EmailAddress = confirmation.Owner.Value;
            ViewBag.Token = token;
            return View(MVC.Authentication.Views.CreatePassword);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up/password")]
        public virtual async Task<ActionResult> Index(CreateLocalMembership command, string emailAddress = null)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null || string.IsNullOrWhiteSpace(emailAddress))
                return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.EmailAddress = emailAddress;
                ViewBag.Token = command.Token;
                return View(MVC.Authentication.Views.CreatePassword, command);
            }

            await _commands.Execute(command);

            await _commands.Execute(new SignIn
            {
                UserName = command.UserName,
                Password = command.Password
            });
            return RedirectToAction(MVC.Home.Index());
        }

        [HttpPost, Route("sign-up/password/validate/{fieldName?}")]
        public virtual ActionResult Validate(CreateLocalMembership command, string fieldName = null)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null)
            {
                Response.StatusCode = 400;
                return Json(null);
            }

            var result = new ValidatedFields(ModelState, fieldName);

            //ModelState[command.PropertyName(x => x.UserName)].Errors.Clear();
            //result = new ValidatedFields(ModelState, fieldName);

            return new CamelCaseJsonResult(result);
        }
    }
}
