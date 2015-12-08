using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Domain.Order;
using Domain.Order.Commands;
using Domain.Order.Queries;
using Domain.Order.Queries.Models;
using infrastructure.CQRS;

namespace Admin.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IQueryChannel _queryChannel;
        private readonly ICommandDispatcher _commandDispatcher;

        public OrdersController(IQueryChannel queryChannel, ICommandDispatcher commandDispatcher)
        {
            _queryChannel = queryChannel;
            _commandDispatcher = commandDispatcher;
        }

        // GET: Orders
        public async Task<ActionResult> Index(string orderStatus = "PaymentSucccessful")
        {
            var orders = await _queryChannel.QueryAsync(new GetOrderDetailsByStatusQuery() {Status = orderStatus});
            if (orders == null)
            {
                orders = new List<OrderDetailsViewModel>();
            }
            ViewBag.OrderStatus = orderStatus;
            return View(orders);
        }

        public async Task<ActionResult> Detail(Guid orderId)
        {
            var order = await _queryChannel.QueryAsync(new GetOrderDetailByAggregateId() { AggregateId = orderId });
            return View(order);
        }

        
        public async Task<ActionResult> PickedUp(Guid orderId)
        {
            var order = await _queryChannel.QueryAsync(new GetOrderDetailByAggregateId() { AggregateId = orderId });
            await
                _commandDispatcher.DispatchAsync(new SetOrderStatusCommand()
                {
                    OrderId = order.Order.Id,
                    OrderStatus = OrderStatus.PickedUp
                });
            return RedirectToAction("Index", new { orderStatus = OrderStatus.PickedUp });
        }

        public async Task<ActionResult> Delivered(Guid orderId)
        {
            var order = await _queryChannel.QueryAsync(new GetOrderDetailByAggregateId() { AggregateId = orderId });
            await
                _commandDispatcher.DispatchAsync(new SetOrderStatusCommand()
                {
                    OrderId = order.Order.Id,
                    OrderStatus = OrderStatus.Delivered
                });
            return RedirectToAction("Index", new { orderStatus = OrderStatus.Delivered });
        }

        

        public async Task<ActionResult> RestaurantAccepted(Guid orderId)
        {
            var order = await _queryChannel.QueryAsync(new GetOrderDetailByAggregateId() {AggregateId = orderId});
            await
                _commandDispatcher.DispatchAsync(new SetOrderStatusCommand()
                {
                    OrderId = order.Order.Id,
                    OrderStatus = OrderStatus.RestaurantAccepted
                });
            return RedirectToAction("Index", new {orderStatus = OrderStatus.RestaurantAccepted});
        }
        
    }
}