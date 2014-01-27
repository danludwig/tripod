using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class UserController : Controller
    {
        private readonly IProcessQueries _queries;

        public UserController(IProcessQueries queries)
        {
            _queries = queries;
        }

        [HttpGet, Route("users/{userId:int}")]
        public virtual async Task<ActionResult> ById(int userId)
        {
            var view = await _queries.Execute(new UserViewBy(userId));
            return View(MVC.Security.Views.User);
        }

        [HttpPut, Route("users/{userId:int}/name")]
        public virtual async Task<ActionResult> ChangeName(int userId)
        {
            return View(MVC.Security.Views.User);
        }
    }
}