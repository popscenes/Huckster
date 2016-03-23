using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Domain.Customer.Queries;
using Domain.Enquiry.Quiries;
using infrastructure.CQRS;
using infrastructure.Messaging;

namespace Admin.Controllers
{
    public class CustomerController : Controller
    {
        private readonly IQueryChannel _queryChannel;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMessageBus _messageBus;

        public CustomerController(IQueryChannel queryChannel, ICommandDispatcher commandDispatcher, IMessageBus messageBus)
        {
            _queryChannel = queryChannel;
            _commandDispatcher = commandDispatcher;
            _messageBus = messageBus;
        }

        // GET: Customer
        public async Task<ActionResult> Index(string searchText = "")
        {
            ViewBag.SearchText = searchText;
            var customers = await _queryChannel.QueryAsync(new GetCustomersQuery()
            {
                Searchtext = searchText
            });
            return View(customers);
        }

        public async Task<ActionResult> Details(Guid id)
        {
            var customerDetail = await _queryChannel.QueryAsync(new GetCustomerDetilQuery()
            {
                AggregateRootId = id
            });

            return View(customerDetail);
        }

        public async Task<ActionResult> Enquiry()
        {
            var enquiries = await _queryChannel.QueryAsync(new GetEnquiriesQuery());

            return View(enquiries);
        }
    }
}