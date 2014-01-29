using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class SignOutController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public SignOutController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-out")]
        public virtual ActionResult Index()
        {
            _commands.Execute(new SignOut());
            Response.ClientCookie(null, _queries);
            return RedirectToAction(MVC.Home.Index());
        }
    }
}