using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class SignOutController : Controller
    {
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public SignOutController(IProcessCommands commands)
        {
            _commands = commands;
        }

        [Authorize]
        [HttpPost, Route("sign-out")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult Index()
        {
            _commands.Execute(new SignOut());
            return RedirectToAction(MVC.Home.Index());
        }
    }
}