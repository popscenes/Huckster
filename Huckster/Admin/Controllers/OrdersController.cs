using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Application.Payment;
using Domain.Order;
using Domain.Order.Commands;
using Domain.Order.Queries;
using Domain.Order.Queries.Models;
using infrastructure.CQRS;

namespace Admin.Controllers
{
    [Authorize(Roles = "Delivery,Admin")]
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
            var order = await _queryChannel.QueryAsync(new GetOrderAdminDetailByAggregateId() { AggregateId = orderId });
            return View(order);
        }

        public async Task<ActionResult>  Cancelled(Guid orderId, bool redirectToDetailPage = false)
        {
            var order = await _queryChannel.QueryAsync(new GetOrderDetailByAggregateId() { AggregateId = orderId });
            await _commandDispatcher.DispatchAsync(new SetOrderStatusCommand()
                {
                    OrderId = order.Order.Id,
                    OrderStatus = OrderStatus.Cancelled
                });

            await _commandDispatcher.DispatchAsync(new RefundOrderCommand() {OrderId = order.Order.Id, OrderAggregateRootId = order.Order.AggregateRootId});

            if (redirectToDetailPage)
            {
                return RedirectToAction("Detail", new { orderId = order.Order.AggregateRootId });
            }
            return RedirectToAction("Index", new { orderStatus = OrderStatus.PaymentSucccessful });
        }


        public async Task<ActionResult> PickedUp(Guid orderId, bool redirectToDetailPage = false)
        {
            var order = await _queryChannel.QueryAsync(new GetOrderDetailByAggregateId() { AggregateId = orderId });
            await
                _commandDispatcher.DispatchAsync(new SetOrderStatusCommand()
                {
                    OrderId = order.Order.Id,
                    OrderStatus = OrderStatus.PickedUp
                });

            if (redirectToDetailPage)
            {
                return RedirectToAction("Detail", new { orderId = order.Order.AggregateRootId });
            }
            return RedirectToAction("Index", new { orderStatus = OrderStatus.RestaurantAccepted });
        }

        public async Task<ActionResult> Delivered(Guid orderId, bool redirectToDetailPage = false)
        {
            var order = await _queryChannel.QueryAsync(new GetOrderDetailByAggregateId() { AggregateId = orderId });
            await
                _commandDispatcher.DispatchAsync(new SetOrderStatusCommand()
                {
                    OrderId = order.Order.Id,
                    OrderStatus = OrderStatus.Delivered
                });

            if (redirectToDetailPage)
            {
                return RedirectToAction("Detail", new { orderId = order.Order.AggregateRootId });
            }
            return RedirectToAction("Index", new { orderStatus = OrderStatus.PickedUp });
        }

        

        public async Task<ActionResult> RestaurantAccepted(Guid orderId, bool redirectToDetailPage = false)
        {
            var order = await _queryChannel.QueryAsync(new GetOrderDetailByAggregateId() {AggregateId = orderId});
            await
                _commandDispatcher.DispatchAsync(new SetOrderStatusCommand()
                {
                    OrderId = order.Order.Id,
                    OrderStatus = OrderStatus.RestaurantAccepted
                });

            if (redirectToDetailPage)
            {
                return RedirectToAction("Detail", new {orderId = order.Order.AggregateRootId});
            }
            return RedirectToAction("Index", new {orderStatus = OrderStatus.PaymentSucccessful});
        }
        
    }
}