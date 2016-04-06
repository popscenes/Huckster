using System.Web.Mvc;

namespace RestaurantPortal.Controllers
{
    [Authorize(Roles = "Orders")]
    public class OrdersController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}