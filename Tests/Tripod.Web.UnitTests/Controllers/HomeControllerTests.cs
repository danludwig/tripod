using System.Web.Mvc;
using Should;
using Xunit;

namespace Tripod.Web.Controllers
{
    public class HomeControllerTests
    {
        [Fact]
        public void About_SetsViewBag_Message()
        {
            var controller = new HomeController();
            ViewResult result = controller.About();
            result.ShouldNotBeNull();
            var viewBagMessage = result.ViewBag.Message as string;
            viewBagMessage.ShouldNotBeNull();
            viewBagMessage.ShouldEqual("Your application description page.");
        }
    }
}
