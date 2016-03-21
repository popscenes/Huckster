using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Domain.Enquiry;
using Domain.Enquiry.Commands;
using infrastructure.CQRS;

namespace WebSite.Controllers
{
    public class EnquiryController : Controller
    {
        private readonly ICommandDispatcher _commandDispatcher;

        public EnquiryController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        // GET: Enquiry
        [HttpGet]
        public ActionResult Index(string subject)
        {
            var enquiry = new Enquiry() {Subject = subject};

            return View(enquiry);
        }

        [HttpPost]
        public async Task<ActionResult> Index(Enquiry enquiry)
        {
            await _commandDispatcher.DispatchAsync(new NewEnquiryCommand()
            {
                Enquiry = enquiry
            });

            return View("EnquiryComplete");
        }
    }
}