using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Application.Payment;
using Domain.Customer;
using Domain.Customer.Commands;
using Domain.Customer.Queries;
using Domain.Order.Commands;
using Domain.Order.Messages;
using Domain.Order.Queries;
using Domain.Restaurant;
using Domain.Restaurant.Queries;
using Domain.Shared;
using infrastructure.CQRS;
using infrastructure.Messaging;
using infrastructure.Messaging.Azure;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        private readonly IQueryChannel _queryChannel;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IMessageBus _messageBus;
        private readonly PaypalService _paypalService;
        // GET: Restaurant
        public OrderController(IQueryChannel queryChannel, PaypalService paypalService, ICommandDispatcher commandDispatcher, IMessageBus messageBus)
        {
            _queryChannel = queryChannel;
            _commandDispatcher = commandDispatcher;
            _messageBus = messageBus;
            _paypalService = paypalService;
        }


        [HttpGet]
        [Route("order/customerdetails/{aggregateid}")]
        public async Task<ActionResult> CustomerDetails(Guid aggregateid)
        {

            var order = await _queryChannel.QueryAsync(new GetOrderDetailByAggregateId { AggregateId = aggregateid });
            var model = new OrderCustomerDetails();
            model.Id = order.Order.Id;
            model.Address = new Address
            {
                Suburb = order.DeliverySuburb.Suburb,
                State = order.DeliverySuburb.State,
                Postcode = order.DeliverySuburb.Postcode
            };

            return View(model);
        }

        [HttpPost]
        [Route("order/customerdetails/{aggregateid}")]
        public async Task<ActionResult> CustomerDetails(Guid aggregateid, OrderCustomerDetails orderCustomerDetails)
        {
            //hack for now
            orderCustomerDetails.Address.City = "Melbourne";

            var customer = await _queryChannel.QueryAsync(new GetCustomerByEmailorMobile()
            {
                Email = orderCustomerDetails.CustomerEmail,
                Mobile = orderCustomerDetails.CustomerMobile,
            });
            if (customer == null)
            {
                var newCustomer = new Customer()
                {
                    AggregateRootId = Guid.NewGuid(),
                    Email = orderCustomerDetails.CustomerEmail,
                    Mobile = orderCustomerDetails.CustomerMobile,
                    Name = ""                   
                };
                await _commandDispatcher.DispatchAsync(new CreateCustomerCommand()
                {
                    NewCustomer = newCustomer,
                    Address = orderCustomerDetails.Address
                });
                customer = newCustomer;
            }

            await _commandDispatcher.DispatchAsync(new UpdateOrderCustomerAndAddress()
            {
                OrderId = orderCustomerDetails.Id,
                Customer = customer,
                Address = orderCustomerDetails.Address
            });
            return RedirectToAction("Finalise");
        }

        [HttpGet]
        [Route("order/finalise/{aggregateid}")]
        public async Task<ActionResult> Finalise(Guid aggregateid)
        {
            
            var order = await _queryChannel.QueryAsync(new GetOrderDetailByAggregateId {AggregateId = aggregateid });
            return View(order);
        }

        [HttpGet]
        [Route("order/complete/{aggregateid}")]
        public async Task<ActionResult> Complete(Guid aggregateid)
        {
            
            var order = await _queryChannel.QueryAsync(new GetOrderByAggregateId() { AggregateId = aggregateid });
            _messageBus.SendMessage(new OrderCompleteMessage() { OrderAggregateRootId = aggregateid });
            return View(order);
        }

        [HttpGet]
        [Route("order/paypal-success/{aggregateid}")]
        public async Task<ActionResult> PaypalSuccess(Guid aggregateid, string paymentId, string PayerID)
        {
            var order = await _queryChannel.QueryAsync(new GetOrderByAggregateId() { AggregateId = aggregateid });
            var paymentEvent = await _paypalService.ExecutePayment(paymentId, PayerID);
            await _commandDispatcher.DispatchAsync(new OrderPaymentSuccessCommand() { Order = order, Payment = paymentEvent });
            return RedirectToAction("Complete");
        }

        [HttpGet]
        [Route("order/paypal-fail/{aggregateid}")]
        public async Task<ActionResult> PaypalFail(Guid aggregateid)
        {
            
            var order = await _queryChannel.QueryAsync(new GetOrderByAggregateId() { AggregateId = aggregateid });
            await _commandDispatcher.DispatchAsync(new OrderPaymentFailCommand() { Order = order});
            return RedirectToAction("Complete");
        }
    }
}