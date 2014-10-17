using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class SignUpVerifySecretController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public SignUpVerifySecretController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("sign-up/register/{ticket}")]
        public virtual async Task<ActionResult> Index(string ticket, string returnUrl)
        {
            var confirmation = await _queries.Execute(new EmailVerificationBy(ticket)
            {
                EagerLoad = new Expression<Func<EmailVerification, object>>[]
                {
                    x => x.EmailAddress,
                }
            });
            if (confirmation == null) return HttpNotFound();

            // todo: confirmation token must not be redeemed, expired, or for different purpose

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ActionUrl = Url.Action(MVC.SignUpVerifySecret.Post());
            ViewBag.Ticket = ticket;
            ViewBag.Purpose = EmailVerificationPurpose.CreateLocalUser;
            if (Session.VerifyEmailTickets().Contains(ticket))
                ViewBag.EmailAddress = confirmation.EmailAddress.Value;
            return View(MVC.Security.Views.SignUp.VerifySecret);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up/register/{ticket}/validate/{fieldName?}")]
        public virtual ActionResult Validate(VerifyEmailSecret command, string fieldName = null)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));
            if (command == null)
            {
                Response.StatusCode = 400;
                return Json(null);
            }

            var result = new ValidatedFields(ModelState, fieldName);

            //ModelState[command.PropertyName(x => x.EmailAddress)].Errors.Clear();
            //result = new ValidatedFields(ModelState, fieldName);

            return new CamelCaseJsonResult(result);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up/register/{ticket}")]
        public virtual async Task<ActionResult> Post(string ticket, VerifyEmailSecret command, string returnUrl, string emailAddress)
        {
            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ActionUrl = Url.Action(MVC.SignUpVerifySecret.Post());
                ViewBag.Ticket = ticket;
                ViewBag.Purpose = EmailVerificationPurpose.CreateLocalUser;
                if (Session.VerifyEmailTickets().Contains(ticket))
                    ViewBag.EmailAddress = emailAddress;
                return View(MVC.Security.Views.SignUp.VerifySecret, command);
            }

            await _commands.Execute(command);

            return RedirectToAction(await MVC.SignUpCreateUser.Index(command.Token, ticket, returnUrl));
        }
    }
}