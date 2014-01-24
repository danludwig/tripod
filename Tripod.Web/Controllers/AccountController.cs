using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    [Authorize]
    public partial class AccountController : Controller
    {
        private readonly IProcessQueries _queries;

        [UsedImplicitly]
        public AccountController(IProcessQueries queries)
        {
            _queries = queries;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        [HttpGet, Route("account/manage")]
        public virtual async Task<ActionResult> Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = await _queries.Execute(new UserHasLocalMembership(User));
            ViewBag.ReturnUrl = Url.Action(MVC.Account.Manage());
            return View();
        }
    }
}