using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class EmailAddressesController : Controller
    {
        private readonly IProcessCommands _commands;

        public EmailAddressesController(IProcessCommands commands)
        {
            _commands = commands;
        }

        [HttpGet, Route("sign-up")]
        public virtual ActionResult SignUp(string token = null)
        {
            return View(MVC.Authentication.Views.SignUp);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up")]
        public virtual async Task<ActionResult> SignUp(SendConfirmationEmail command)
        {
            if (!ModelState.IsValid) return View(MVC.Authentication.Views.SignUp, command);

            // todo: what if email matches a user account? error or redirect?

            await _commands.Execute(command);

            return View(MVC.Authentication.Views.SignUp, command);
        }

        [HttpPost, Route("sign-up/validate/{fieldName?}")]
        public virtual ActionResult SignUpValidate(SendConfirmationEmail command, string fieldName = null)
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

        [HttpGet]
        [Route("sign-up/confirm/{ticket:Guid}")]
        [Route("sign-up/confirm")]
        public virtual ActionResult Confirm(Guid? ticket = null, string token = null)
        {
            return View(MVC.Authentication.Views.SignUp);
        }
    }
}