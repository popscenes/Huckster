using infrastructure.Utility;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var testRestaurantId = ConfigurationManager.AppSettings["TestRestaurantId"];
            if (testRestaurantId.IsNullOrWhiteSpace())
            {
                testRestaurantId = "1";
            }

            ViewBag.TestRestaurantId = testRestaurantId;
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            

            return View();
        }
    }
}