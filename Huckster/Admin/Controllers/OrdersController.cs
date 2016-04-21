using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Admin.Models;
using Application.Payment;
using Domain.Order;
using Domain.Order.Commands;
using Domain.Order.Messages;
using Domain.Order.Queries;
using Domain.Order.Queries.Models;
using infrastructure.CQRS;
using infrastructure.Messaging;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace Admin.Controllers
{
    [Authorize(Roles = "Delivery,Admin")]
    public class OrdersController : Controller
    {
        private readonly IQueryChannel _queryChannel;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMessageBus _messageBus;

        public OrdersController(IQueryChannel queryChannel, ICommandDispatcher commandDispatcher, IMessageBus messageBus)
        {
            _queryChannel = queryChannel;
            _commandDispatcher = commandDispatcher;
            _messageBus = messageBus;
        }

        // GET: Orders
        public async Task<ActionResult> Index(string orderStatus = "PaymentSucccessful", string deliveryUser = "")
        {
            var orders = await _queryChannel.QueryAsync(new GetOrderDetailsByStatusQuery() {Status = orderStatus, DeliveryUserId = deliveryUser}) ??
                         new List<OrderAdminDetailsViewModel>();

            ViewBag.DeliveryUsers = GetDeliveryUsers();
            ViewBag.OrderStatus = orderStatus;

            return View(orders);
        }

        protected List<SelectListItem> GetDeliveryUsers()
        {
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var _roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(HttpContext.GetOwinContext().Get<ApplicationDbContext>()));

            var deliveryRole = _roleManager.Roles.FirstOrDefault(_ => _.Name.Equals("Delivery"));
            var deliveryUsers = userManager.Users.Where(_ => _.Roles.Select(r => r.RoleId).Contains(deliveryRole.Id));
            var deliverSelectList = deliveryUsers.Select(_ => new SelectListItem() { Text = _.UserName, Value = _.Id });

            return deliverSelectList.ToList();
        }

        public async Task<ActionResult> Detail(Guid orderId)
        {
            ViewBag.DeliveryUsers = GetDeliveryUsers();
            var order = await _queryChannel.QueryAsync(new GetOrderAdminDetailByAggregateId() { AggregateId = orderId });
            return View(order);
        }

        public async Task<ActionResult> SetDeliveryDetails(Guid orderId, string deliveryUser)
        {
            try
            {
                //var pickUpTimespan = DateTime.ParseExact(pickUpTime, "h:mmtt", CultureInfo.InvariantCulture);

                var order = await _queryChannel.QueryAsync(new GetOrderDetailByAggregateId() { AggregateId = orderId });

                //var pickUpdatetime = order.Order.DeliveryTime.Date.Add(pickUpTimespan.TimeOfDay);

                await _commandDispatcher.DispatchAsync(new SetDeliveryDetailsCommand()
                {
                    OrderId = order.Order.Id,
                    PickUpDateTime = order.Order.PickUpTime.Value,
                    DeliveryUserId = deliveryUser
                });
                _messageBus.SendMessage(new OrderAssignedMessage() { OrderAggregateRootId = order.Order.AggregateRootId, AssignedUserId =  deliveryUser});
                Audit(order.Order.AggregateRootId, "SetDeliveryDetails");
            }
            catch (Exception e)
            {
                
                throw;
            }
           
            return RedirectToAction("Detail", new {orderId = orderId});
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

            Audit(order.Order.AggregateRootId, "Cancelled");
            _messageBus.SendMessage(new OrderDeclinedMessage() { OrderAggregateRootId = order.Order.AggregateRootId });


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

            Audit(order.Order.AggregateRootId, "PickedUp");
            _messageBus.SendMessage(new OrderPickUpMessage() { OrderAggregateRootId = order.Order.AggregateRootId });


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

            Audit(order.Order.AggregateRootId, "Delivered");

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

                Audit(order.Order.AggregateRootId, "RestaurantAccepted");
             _messageBus.SendMessage(new OrderAcceptedMessage() { OrderAggregateRootId = order.Order.AggregateRootId });

            if (redirectToDetailPage)
            {
                return RedirectToAction("Detail", new {orderId = order.Order.AggregateRootId});
            }
            return RedirectToAction("Index", new {orderStatus = OrderStatus.PaymentSucccessful});
        }

        private async void Audit(Guid orderId, string action)
        {
            var userName = this.HttpContext.User.Identity.Name;
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