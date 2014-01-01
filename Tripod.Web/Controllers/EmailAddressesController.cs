using System.Web.Mvc;
using Tripod.Domain.Security;

namespace Tripod.Web.Controllers
{
    public partial class EmailAddressesController : Controller
    {
        [HttpGet, Route("account/register2")]
        public virtual ViewResult Index()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost, Route("account/register2")]
        public virtual ActionResult SendEmail(SendConfirmationEmail command)
        {
            var isValid = ModelState.IsValid;
            return View(MVC.EmailAddresses.Views.Index, command);
        }

        [HttpPost, Route("account/register2/validate")]
        public virtual JsonResult ValidateSendEmail(SendConfirmationEmail command)
        {
            return Json("Here is a custom error message for Email address.");
        }
    }
}