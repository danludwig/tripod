using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class SignOnSecretController : Controller
    {
        private readonly IProcessQueries _queries;
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public SignOnSecretController(IProcessQueries queries, IProcessCommands commands)
        {
            _queries = queries;
            _commands = commands;
        }

        [HttpGet, Route("sign-on/{ticket}")]
        public virtual async Task<ActionResult> Index(string ticket, string returnUrl)
        {
            // todo: make sure we still have a remote login

            var confirmation = await _queries.Execute(new EmailConfirmationBy(ticket)
            {
                EagerLoad = new Expression<Func<EmailConfirmation, object>>[]
                {
                    x => x.Owner,
                }
            });
            if (confirmation == null) return HttpNotFound();

            // todo: confirmation token must not be redeemed, expired, or for different purpose

            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ActionUrl = Url.Action(MVC.SignOnSecret.Index(ticket, null));
            ViewBag.Ticket = ticket;
            ViewBag.Purpose = EmailConfirmationPurpose.CreateRemoteUser;
            if (Session.ConfirmEmailTickets().Contains(ticket))
                ViewBag.EmailAddress = confirmation.Owner.Value;
            return View(MVC.Security.Views.SignOnSecret);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-on/{ticket}")]
        public virtual async Task<ActionResult> Index(string ticket, VerifyConfirmEmailSecret command, string returnUrl, string emailAddress)
        {
            // todo: make sure we still have a remote login

            //System.Threading.Thread.Sleep(new Random().Next(5000, 5001));

            if (command == null) return View(MVC.Errors.Views.BadRequest);

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ActionUrl = Url.Action(MVC.SignOnSecret.Index(ticket, null));
                ViewBag.Ticket = ticket;
                ViewBag.Purpose = EmailConfirmationPurpose.CreateRemoteUser;
                if (Session.ConfirmEmailTickets().Contains(ticket))
                    ViewBag.EmailAddress = emailAddress;
                return View(MVC.Security.Views.SignOnSecret, command);
            }

            await _commands.Execute(command);

            return HttpNotFound();
            //return RedirectToAction(await MVC.SignUpUser.Index(command.Token, returnUrl));
        }
    }
}
