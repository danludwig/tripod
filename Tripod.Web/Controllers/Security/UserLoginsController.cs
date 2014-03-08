using System.Web.Mvc;

namespace Tripod.Web.Controllers
{
    public partial class UserLoginsController : Controller
    {
        private readonly IProcessQueries _queries;

        public UserLoginsController(IProcessQueries queries)
        {
            _queries = queries;
        }

        [HttpGet, Route("settings/logins")]
        public virtual ActionResult Index()
        {
            return View(MVC.Security.Views.UserLogins);
        }
	}
}