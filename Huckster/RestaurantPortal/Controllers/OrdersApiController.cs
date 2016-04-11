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
using infrastructure.Messaging;
using Domain.Order.Messages;
using Domain.Order.Commands;

namespace RestaurantPortal.Controllers
{
    [Authorize(Roles = "Orders")]
    public class OrdersApiController : ApiController
    {
        private readonly IQueryChannel _queryChannel;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMessageBus _messageBus;

        public OrdersApiController(IQueryChannel queryChannel, ICommandDispatcher commandDispatcher, IMessageBus messageBus)
        {
            _queryChannel = queryChannel;
            _commandDispatcher = commandDispatcher;
            _messageBus = messageBus;
        }

        [HttpGet]
        [Route("api/orders")]
        public async Task<IHttpActionResult> GetOrders(string orderStatus = "PaymentSucccessful")
        {
            var userId = User.Identity.GetUserId();
            var orders = await _queryChannel.QueryAsync(new GetOrderDetailsByUserStatusQuery() { Status = orderStatus, UserId = userId }) ??
                         new List<OrderAdminDetailsViewModel>();

            return Ok(orders);
        }

        public class RestaurantOrderAcceptRequestModel
        {
            public Guid OrderId { get; set; } 
            public TimeSpan PickUpTime { get; set; }
        }

        [HttpPost]
        [Route("api/orders/restaurantOrderAccept")]
        public async Task<IHttpActionResult> RestaurantOrderAccept(RestaurantOrderAcceptRequestModel requestModel)
        {
            var userId = User.Identity.GetUserId();
            await _commandDispatcher.DispatchAsync(new RestaurantOrderAcceptCommand() {
                OrderAggregateRootId = requestModel.OrderId,
                pickUpTime = requestModel.PickUpTime
            });

            _messageBus.SendMessage(new RestaurantOrderAcceptMessage() { OrderAggregateRootId = requestModel.OrderId});
            Audit(requestModel.OrderId = requestModel.OrderId, "RestaurantOrderAccept");
            return Ok();
        }

        private async void Audit(Guid orderId, string action)
        {
            var userName = User.Identity.GetUserName();
            await
                _commandDispatcher.DispatchAsync(new InsertOrderAuditCommand()
                {
                    Action = action,
                    UserName = action,
                    OrderAggregateRoot = orderId
                });
        }


    }
}
