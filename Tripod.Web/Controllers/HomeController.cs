using System.Web.Mvc;

namespace Tripod.Web.Controllers
{
    public partial class HomeController : Controller
    {
        [HttpGet, Route("")]
        public virtual ActionResult Index()
        {
            return View();
        }

        [HttpGet, Route("about")]
        public virtual ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [HttpGet, Route("contact")]
        public virtual ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}