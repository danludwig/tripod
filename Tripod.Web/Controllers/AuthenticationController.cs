using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IProcessCommands _commands;

        public AuthenticationController(IProcessCommands commands)
        {
            _commands = commands;
        }

        [AllowAnonymous]
        [HttpGet, Route("account/login")]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("~/Views/Account/Login.cshtml");
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/login")]
        public async Task<ActionResult> Login(SignIn command, string returnUrl)
        {
            if (!ModelState.IsValid) return View("~/Views/Account/Login.cshtml", command);
            await _commands.Execute(command);
            return RedirectToLocal(returnUrl);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}