using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class SignOnVerifySecretController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public SignOnVerifySecretController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("sign-on/register/{ticket}")]
        public virtual async Task<ActionResult> Index(string ticket, string returnUrl)
        {
            // todo: make sure we still have a remote login

            var verification = await _queries.Execute(new EmailVerificationBy(ticket)
            {
                EagerLoad = new Expression<Func<EmailVerification, object>>[]
                {
                    x => x.EmailAddress,
                }
            });
            if (verification == null) return HttpNotFound();

            // todo: verification token must not be redeemed, expired, or for different purpose

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ActionUrl = Url.Action(MVC.SignOnVerifySecret.Post(ticket, null, null, null));
            ViewBag.Ticket = ticket;
            ViewBag.Purpose = EmailVerificationPurpose.CreateRemoteUser;
            if (Session.VerifyEmailTickets().Contains(ticket))
                ViewBag.EmailAddress = verification.EmailAddress.Value;
            return View(MVC.Security.Views.SignOn.VerifySecret);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-on/register/{ticket}")]
        public virtual async Task<ActionResult> Post(string ticket, VerifyEmailSecret command, string returnUrl, string emailAddress)
        {
            // todo: make sure we still have a remote login

            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ActionUrl = Url.Action(MVC.SignOnVerifySecret.Post(ticket, null, null, null));
                ViewBag.Ticket = ticket;
                ViewBag.Purpose = EmailVerificationPurpose.CreateRemoteUser;
                if (Session.VerifyEmailTickets().Contains(ticket))
                    ViewBag.EmailAddress = emailAddress;
                return View(MVC.Security.Views.SignOn.VerifySecret, command);
            }

            await _commands.Execute(command);

            return RedirectToAction(await MVC.SignOnCreateUser.Index(command.Token, ticket, returnUrl));
        }
    }
}