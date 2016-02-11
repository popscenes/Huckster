using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Order.Messages;
using infrastructure.Messaging.Azure;

namespace WebJobSite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult TestOrdrCompleteEmail()
        {
            var messageBus = new WebJobMessageBus();
            //messageBus.SendMessage(new OrderCompleteMessage() {OrderAggregateRootId = "1"});
            return RedirectToAction("Index");
        }
    }
}