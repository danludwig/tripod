using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class SignOnUserController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public SignOnUserController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("sign-on/register")]
        public virtual async Task<ActionResult> Index(string token, string returnUrl)
        {
            var userToken = await _queries.Execute(new EmailConfirmationUserToken(token));
            if (userToken == null) return HttpNotFound();
            var confirmation = await _queries.Execute(new EmailConfirmationBy(userToken.Value));
            if (confirmation == null) return HttpNotFound();

            // todo: confirmation cannot be expired, redeemed, or for different purpose

            ViewBag.Token = token;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.EmailAddress = confirmation.Owner.Value;
            return View(MVC.Security.Views.SignOnUser);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-on/register")]
        public virtual async Task<ActionResult> Index(CreateLocalMembership command, string returnUrl, string emailAddress)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null || string.IsNullOrWhiteSpace(emailAddress))
                return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.Token = command.Token;
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.EmailAddress = emailAddress;
                return View(MVC.Security.Views.SignOnUser, command);
            }

            await _commands.Execute(command);

            await _commands.Execute(new SignIn
            {
                UserName = command.UserName,
                Password = command.Password
            });
            return this.RedirectToLocal(returnUrl);
        }

        //[HttpPost, Route("sign-up/password/validate/{fieldName?}")]
        //public virtual ActionResult Validate(CreateLocalMembership command, string fieldName = null)
        //{
        //    //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

        //    if (command == null)
        //    {
        //        Response.StatusCode = 400;
        //        return Json(null);
        //    }

        //    var result = new ValidatedFields(ModelState, fieldName);

        //    //ModelState[command.PropertyName(x => x.UserName)].Errors.Clear();
        //    //result = new ValidatedFields(ModelState, fieldName);

        //    return new CamelCaseJsonResult(result);
        //}
    }
}
