using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Application.Payment;
using Domain.Order;
using Domain.Order.Commands;
using Domain.Order.Queries;
using infrastructure.CQRS;

namespace WebSite.Controllers
{
    public class OrderApiController : ApiController
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryChannel _queryChannel;
        private readonly StripeService _stripeService;
        private readonly PaypalService _paypalService;

        public OrderApiController(ICommandDispatcher commandDispatcher, IQueryChannel queryChannel, StripeService stripeService, PaypalService paypalService)
        {
            _commandDispatcher = commandDispatcher;
            _queryChannel = queryChannel;
            _stripeService = stripeService;
            _paypalService = paypalService;
        }

        [HttpPost]
        [Route("api/Order/PlaceOrder")]
        public async Task<IHttpActionResult> PlaceOrder(PlaceOrderVieModel viewModel)
        {
            viewModel.order.AggregateRootId = Guid.NewGuid();
            await _commandDispatcher.DispatchAsync(new PlaceOrderCommand()
            {
                Order = viewModel.order,
                OrderItems = viewModel.orderItems
            });
            return Ok(viewModel.order.AggregateRootId);
        }

        [HttpPost]
        [Route("api/Order/Payment/Stripe")]
        public async Task<IHttpActionResult> PaymentStripe(StripePaymentVieModel paymentModel)
        {
            var order = await _queryChannel.QueryAsync(new GetOrderByAggregateId() { AggregateId = paymentModel.OrderId });
            var payment = await _stripeService.CreateCharge(paymentModel.PaymentToken, order.OrderItems.Sum(_ => _.Quantity*_.Price), "Huckster Order");
            await _commandDispatcher.DispatchAsync(new OrderPaymentSuccessCommand() {Order = order, Payment = payment});
            return Ok(order.Id);
        }

        [HttpPost]
        [Route("api/Order/Payment/paypal-redirect")]
        public async Task<IHttpActionResult> PaypalRedirect(PaypalPaymentVieModel paymentModel)
        {
            var order = await _queryChannel.QueryAsync(new GetOrderByAggregateId() { AggregateId = paymentModel.OrderId });

            var failUrl = String.Format(ConfigurationManager.AppSettings["PaypalFailurl"], order.AggregateRootId);
            var successUrl = String.Format(ConfigurationManager.AppSettings["PaypalSuccessurl"], order.AggregateRootId);

            var redirectUrl = await _paypalService.GetRedirectUrl(order.OrderItems.Sum(_ => _.Quantity * _.Price), "Hickster Order", successUrl, failUrl);
            return Ok(redirectUrl);
        }
    }

    public class PlaceOrderVieModel
    {
        public Order order { get; set; }
        public List<OrderItem> orderItems { get; set; }
    }

    public class PaypalPaymentVieModel
    {
        public Guid OrderId { get; set; }
    }

    public class StripePaymentVieModel
    {
        public string PaymentToken { get; set; }
        public Guid OrderId { get; set; }
    }
}
