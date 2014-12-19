using System.Web.Mvc;

namespace Tripod.Web.Controllers
{
    public partial class GuestbookController : Controller
    {
        [HttpGet, Route("guestbook")]
        public virtual ActionResult Get()
        {
            return View(MVC.Guestbook.Views.Index);
        }

        [HttpPost, Route("guestbook")]
        public virtual ActionResult Post(string text)
        {
            return View(MVC.Guestbook.Views.Index);
        }
    }
}