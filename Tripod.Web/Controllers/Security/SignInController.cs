using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class SignInController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public SignInController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("sign-in")]
        public virtual ActionResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(MVC.Security.Views.SignIn);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-in")]
        public virtual async Task<ActionResult> Index(SignIn command, string returnUrl)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null) return View(MVC.Errors.Views.BadRequest);
            if (!ModelState.IsValid) return View(MVC.Security.Views.SignIn, command);
            await _commands.Execute(command);
            Response.ClientCookie(command.SignedIn.Id, _queries);
            return this.RedirectToLocal(returnUrl, await MVC.User.ById(command.SignedIn.Id));
        }

        [HttpPost, Route("sign-in/validate/{fieldName?}")]
        public virtual ActionResult Validate(SignIn command, string fieldName = null)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));
            if (command == null || command.PropertyName(x => x.Password).Equals(fieldName, StringComparison.OrdinalIgnoreCase))
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