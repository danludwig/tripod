using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

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
        public virtual async Task<ActionResult> ChangeName(ChangeUserName command)
        {
            return View(MVC.Security.Views.User);
        }

        [HttpPost, Route("users/{userId:int}/name/validate")]
        public virtual ActionResult ValidateChangeName(ChangeUserName command)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));
            if (command == null)
            {
                Response.StatusCode = 400;
                return Json(null);
            }

            var result = new ValidatedFields(ModelState);

            //ModelState[command.PropertyName(x => x.UserName)].Errors.Clear();
            //result = new ValidatedFields(ModelState, command.PropertyName(x => x.UserName));

            return new CamelCaseJsonResult(result);
        }
    }
}