using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

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
        public virtual async Task<ActionResult> Index()
        {
            var user = await _queries.Execute(new UserViewBy(User.Identity.GetAppUserId()));

            var model = new LoginSettingsModel
            {
                UserView = user,
            };

            return View(MVC.Security.Views.UserLogins, model);
        }
	}
}