using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Domain.Order.Queries;
using infrastructure.CQRS;

namespace Admin.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IQueryChannel _queryChannel;

        public OrdersController(IQueryChannel queryChannel)
        {
            _queryChannel = queryChannel;
        }

        // GET: Orders
        public async Task<ActionResult> Index(string orderStatus = "PaymentSucccessful")
        {
            var orders = await _queryChannel.QueryAsync(new GetOrderDetailsByStatusQuery() {Status = orderStatus});
            return View(orders);
        }
    }
}