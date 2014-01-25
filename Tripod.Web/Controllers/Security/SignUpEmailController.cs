using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class SignUpEmailController : Controller
    {
        private readonly IProcessCommands _commands;

        [UsedImplicitly]
        public SignUpEmailController(IProcessCommands commands)
        {
            _commands = commands;
        }

        [HttpGet, Route("sign-up")]
        public virtual ViewResult Index(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.ActionUrl = Url.Action(MVC.SignUpEmail.Index());
            ViewBag.Purpose = EmailConfirmationPurpose.CreateLocalUser;
            ViewBag.ConfirmUrlFormat = ConfirmUrlFormat(returnUrl);
            ViewBag.SendFromUrl = SendFromUrl(returnUrl);
            return View(MVC.Security.Views.SignUpEmail);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up")]
        public virtual async Task<ActionResult> Index(SendConfirmationEmail command, string returnUrl, string loginProvider)
        {
            if (command == null || command.Purpose == EmailConfirmationPurpose.Invalid)
            {
                return View(MVC.Errors.Views.BadRequest);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.ActionUrl = Url.Action(MVC.SignUpEmail.Index());
                return View(MVC.Security.Views.SignUpEmail, command);
            }

            command.ConfirmUrlFormat = ConfirmUrlFormat(returnUrl);
            command.SendFromUrl = SendFromUrl(returnUrl);
            await _commands.Execute(command);

            Session.AddConfirmEmailTicket(command.CreatedTicket);

            return RedirectToAction(await MVC.SignUpSecret.Index(command.CreatedTicket, returnUrl));
        }

        [HttpPost, Route("sign-up/validate/{fieldName?}")]
        public virtual ActionResult Validate(SendConfirmationEmail command, string fieldName = null)
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

        private string ConfirmUrlFormat(string returnUrl)
        {
            Debug.Assert(Request.Url != null);
            var encodedUrlFormat = Url.Action(MVC.SignUpUser.Index("{0}", "{1}"));
            var decodedUrlFormat = HttpUtility.UrlDecode(encodedUrlFormat);
            var formattedUrl = string.Format(decodedUrlFormat, "{0}", returnUrl);
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, formattedUrl);
        }

        private string SendFromUrl(string returnUrl)
        {
            Debug.Assert(Request.Url != null);
            var url = Url.Action(MVC.SignUpEmail.Index(returnUrl));
            return string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, url);
        }
    }
}
