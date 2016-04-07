using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Domain.Order.Queries;
using Domain.Order.Queries.Models;
using infrastructure.CQRS;
using Microsoft.AspNet.Identity;

namespace RestaurantPortal.Controllers
{
    [Authorize(Roles = "Orders")]
    public class OrdersApiController : ApiController
    {
        private readonly IQueryChannel _queryChannel;

        public OrdersApiController(IQueryChannel queryChannel)
        {
            _queryChannel = queryChannel;
        }

        [HttpGet]
        [Route("api/orders")]
        public async Task<IHttpActionResult> GetOrders(string orderStatus = "PaymentSucccessful")
        {
            var userId = User.Identity.GetUserId();
            var orders = await _queryChannel.QueryAsync(new GetOrderDetailsByStatusQuery() { Status = orderStatus, DeliveryUserId = "" }) ??
                         new List<OrderAdminDetailsViewModel>();

            return Ok(orders);
        }
    }
}
