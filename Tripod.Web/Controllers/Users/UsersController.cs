using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class UsersController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        public UsersController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        #region Public User Page

        [HttpGet, Route("users/{userId:int}")]
        public virtual async Task<ActionResult> ById(int userId)
        {
            var view = await _queries.Execute(new UserViewBy(userId));
            if (view == null) return HttpNotFound();
            return View(MVC.Users.Views.User, view);
        }

        #endregion
    }
}