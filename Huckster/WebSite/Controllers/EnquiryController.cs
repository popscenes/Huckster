using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Domain.Enquiry;
using Domain.Enquiry.Commands;
using Domain.Enquiry.Messages;
using infrastructure.CQRS;
using infrastructure.Messaging;

namespace WebSite.Controllers
{
    public class EnquiryController : Controller
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMessageBus _messageBus;

        public EnquiryController(ICommandDispatcher commandDispatcher, IMessageBus messageBus)
        {
            _commandDispatcher = commandDispatcher;
            _messageBus = messageBus;
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

            _messageBus.SendMessage(new EmailEnquiryMessage()
            {
                Enquiry = enquiry
            });

            return View("EnquiryComplete");
        }
    }
}