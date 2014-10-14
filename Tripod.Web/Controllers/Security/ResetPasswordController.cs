using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class ResetPasswordController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public ResetPasswordController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("sign-in/password/recover")]
        public virtual async Task<ActionResult> Index(string token, string ticket, string returnUrl)
        {
            var verification = await _queries.Execute(new EmailVerificationBy(ticket));
            if (verification == null) return HttpNotFound();

            // todo: verification cannot be expired, redeemed, or for different purpose

            ViewBag.EmailAddress = verification.EmailAddress.Value;
            ViewBag.Ticket = ticket;
            ViewBag.Token = token;
            ViewBag.ReturnUrl = returnUrl;
            return View(MVC.Security.Views.ResetPassword.ResetPassword);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-in/password/recover/validate/{fieldName?}")]
        public virtual ActionResult Validate(ResetPassword command, string fieldName = null)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null)
            {
                Response.StatusCode = 400;
                return Json(null);
            }

            var result = new ValidatedFields(ModelState, fieldName);

            //ModelState[command.PropertyName(x => x.UserName)].Errors.Clear();
            //result = new ValidatedFields(ModelState, fieldName);

            return new CamelCaseJsonResult(result);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-in/password/recover")]
        public virtual async Task<ActionResult> Post(ResetPassword command, string returnUrl, string emailAddress)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null || string.IsNullOrWhiteSpace(emailAddress))
                return View(MVC.Errors.Views.BadRequest);

            var verification = await _queries.Execute(new EmailVerificationBy(command.Ticket));
            if (verification == null) return HttpNotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.EmailAddress = emailAddress;
                ViewBag.Ticket = command.Ticket;
                ViewBag.Token = command.Token;
                ViewBag.ReturnUrl = returnUrl;
                return View(MVC.Security.Views.ResetPassword.ResetPassword, command);
            }

            await _commands.Execute(command);

            var signIn = new SignIn
            {
                UserNameOrVerifiedEmail = verification.EmailAddress.Value,
                Password = command.Password
            };
            await _commands.Execute(signIn);
            Session.VerifyEmailTickets(null);
            Response.ClientCookie(signIn.SignedIn.Id, _queries);
            return this.RedirectToLocal(returnUrl, await MVC.UserName.Index());
        }
    }
}