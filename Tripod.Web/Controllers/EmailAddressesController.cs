using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Tripod.Domain.Security;
using Tripod.Web.Models;

namespace Tripod.Web.Controllers
{
    public partial class EmailAddressesController : Controller
    {
        private readonly UserManager<User, int> _userManager;
        private readonly IReadEntities _entities;

        public EmailAddressesController(UserManager<User, int> userManager, IReadEntities entities)
        {
            _userManager = userManager;
            _entities = entities;
        }

        private static Guid? _stamp;

        [HttpGet, Route("sign-up")]
        public virtual ActionResult SignUp(string token = null)
        {
            if (_stamp == null)
            {
                _stamp = Guid.NewGuid();
                token = _userManager.UserConfirmationTokens.Generate(new UserToken
                {
                    CreationDate = DateTime.UtcNow,
                    UserId = "test",
                    Value = _stamp.ToString(),
                });
                return RedirectToAction(MVC.EmailAddresses.SignUp(token));
            }
            var userToken = _userManager.UserConfirmationTokens.Validate(token);
            var test = userToken.Value == _stamp.ToString();

            return View(MVC.Authentication.Views.SignUp);
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("sign-up")]
        public virtual ActionResult SignUp(SendConfirmationEmail command)
        {
            // todo: what if email matches a user account? error or redirect?
            //var isValid = ModelState.IsValid;
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
    }
}