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
using Domain.Customer;
using Domain.Customer.Commands;
using Domain.Customer.Queries;
using Domain.Order;
using Domain.Order.Commands;
using Domain.Order.Queries;
using Domain.Order.Queries.Models;
using Domain.Restaurant.Queries;
using Domain.Shared;
using infrastructure.CQRS;
using Omu.ValueInjecter;

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
        [Route("api/Order/DeliveryDetails")]
        public async Task<IHttpActionResult> DeliveryDetails(DeliveryDetailsVieModel viewModel)
        {
            var order = await _queryChannel.QueryAsync(new GetOrderDetailByAggregateId() { AggregateId = viewModel.OrderId });
            var address = order.DeliverAddress;
            if (address == null)
            {
                address = new Address();
            }
            address.InjectFrom(viewModel);
            address.City = "Melbourne";
            await _commandDispatcher.DispatchAsync(new UpdateOrderAddressCommand()
            {
                OrderId = order.Order.Id,
                Address = address
            });

            if (order.Customer != null)
            {
                await _commandDispatcher.DispatchAsync(new UpdateCustomerAddress()
                {
                    Customer = order.Customer,
                    Address = address
                });
            }
            return Ok();
        }

        [HttpPost]
        [Route("api/Order/PersonalDetails")]
        public async Task<IHttpActionResult> PersonalDetails(PersonalDetailsVieModel viewModel)
        {
            var order = await _queryChannel.QueryAsync(new GetOrderDetailByAggregateId() { AggregateId = viewModel.OrderId });
            var customer = order.Customer ?? await _queryChannel.QueryAsync(new GetCustomerByEmailorMobile()
            {
                Email = viewModel.Email,
                Mobile = viewModel.Mobile,
            });

            customer = await UpdateCustomer(customer, viewModel, order);

            await _commandDispatcher.DispatchAsync(new UpdateOrderCustomer()
            {
                Customer = customer,
                Order = order.Order
            });


            return Ok();
        }

        private async Task<Customer> UpdateCustomer(Customer customer, PersonalDetailsVieModel viewModel, OrderDetailsViewModel order)
        {
            if (customer == null)
            {
                var newCustomer = new Customer()
                {
                    AggregateRootId = Guid.NewGuid(),
                    Email = viewModel.Email,
                    Mobile = viewModel.Mobile,
                    Name = viewModel.FirstName + " " + viewModel.LastName
                };
                await _commandDispatcher.DispatchAsync(new CreateCustomerCommand()
                {
                    NewCustomer = newCustomer,
                    Address = order.DeliverAddress
                });
                customer = newCustomer;
            }
            else
            {
                customer.Email = viewModel.Email;
                customer.Mobile = viewModel.Mobile;
                customer.Name = viewModel.FirstName + " " + viewModel.LastName;

                await _commandDispatcher.DispatchAsync(new UpdateCustomer()
                {
                    Customer = customer
                });

                await _commandDispatcher.DispatchAsync(new UpdateCustomerAddress()
                {
                    Customer = customer,
                    Address = order.DeliverAddress
                });
            }
            return customer;
        }


        [HttpPost]
        [Route("api/Order/PlaceOrder")]
        public async Task<IHttpActionResult> PlaceOrder(PlaceOrderVieModel viewModel)
        {
            var restaurant = await _queryChannel.QueryAsync(new GetRestaurantDetailByAggregateRootIdQuery() {AggregateRootId = viewModel.order.RestaurantId});
            viewModel.order.AggregateRootId = Guid.NewGuid();

            var menuItems = restaurant.RestaurantMenu.SelectMany(_ => _.MenuItems);

            // re assign price from server... to avoid client side hacking
            viewModel.orderItems.ForEach((oi) => { oi.Price = menuItems.FirstOrDefault(mi => mi.Id == oi.Id).Price; });


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
            return Ok(order.AggregateRootId);
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


    public class DeliveryDetailsVieModel
    {
        public Guid OrderId { get; set; }
        public string Street { get; set; }
        public string Number { get; set; }
        public string Suburb  { get; set; }
        public string State   { get; set; }
        public string Postcode{ get; set; }
    }

    public class PersonalDetailsVieModel
    {
        public Guid OrderId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
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
