using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class AuthenticationController : Controller
    {
        private readonly IProcessCommands _commands;

        public AuthenticationController(IProcessCommands commands)
        {
            _commands = commands;
        }

        [AllowAnonymous]
        [HttpGet, Route("account/login")]
        public virtual ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(MVC.Account.Views.Login);
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/login")]
        public virtual async Task<ActionResult> Login(SignIn command, string returnUrl)
        {
            if (!ModelState.IsValid) return View(MVC.Account.Views.Login, command);
            await _commands.Execute(command);
            return RedirectToLocal(returnUrl);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(MVC.Home.Index());
        }
    }
}