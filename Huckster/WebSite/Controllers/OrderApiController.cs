using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.UI.WebControls;
using Application.Payment;
using Domain.Customer;
using Domain.Customer.Commands;
using Domain.Customer.Queries;
using Domain.Order;
using Domain.Order.Commands;
using Domain.Order.Queries;
using Domain.Order.Queries.Models;
using Domain.Restaurant.Queries;
using Domain.Restaurant.Queries.Models;
using Domain.Shared;
using infrastructure.CQRS;
using Omu.ValueInjecter;
using MenuItem = Domain.Restaurant.MenuItem;

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
            order.Order.InjectFrom(viewModel);
            await _commandDispatcher.DispatchAsync(new UpdateOrderCommand()
            {
                OrderId = order.Order.Id,
                Order = order.Order
            });


            var address = order.DeliverAddress ?? new Address();
            address.InjectFrom(viewModel);
            address.City = "Melbourne";
            await _commandDispatcher.DispatchAsync(new UpdateOrderAddressCommand()
            {
                OrderAggregateRootId = order.Order.AggregateRootId,
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
        [Route("api/Order/UpdateOrder")]
        public async Task<IHttpActionResult> UpdateOrder(PlaceOrderVieModel viewModel)
        {
            await SetServerSideValues(viewModel);
            await _commandDispatcher.DispatchAsync(new UpdateOrderAntOrderItemsCommand()
            {
                Order = viewModel.order,
                OrderItems = viewModel.orderItems
            });
            return Ok(viewModel.order.AggregateRootId);
        }


        [HttpPost]
        [Route("api/Order/PlaceOrder")]
        public async Task<IHttpActionResult> PlaceOrder(PlaceOrderVieModel viewModel)
        {
            viewModel.order.AggregateRootId = Guid.NewGuid();
            // re assign price from server... to avoid client side hacking
            await SetServerSideValues(viewModel);
            await _commandDispatcher.DispatchAsync(new PlaceOrderCommand()
            {
                Order = viewModel.order,
                OrderItems = viewModel.orderItems
            });
            return Ok(viewModel.order.AggregateRootId);
        }

        private async Task SetServerSideValues(PlaceOrderVieModel viewModel)
        {
            var restaurant = await _queryChannel.QueryAsync(new GetRestaurantDetailByAggregateRootIdQuery()
            {
                GetDeletedmenuItems = true,
                AggregateRootId = viewModel.order.RestaurantId
            });
            var menuItems = restaurant.RestaurantMenu.SelectMany(_ => _.MenuItems).ToList();
            foreach (var orderItem in viewModel.orderItems)
            {
                var matchingMenuItem = menuItems.FirstOrDefault(mi => mi.Id == orderItem.MenuItemKey);
                if (matchingMenuItem == null)
                {
                    throw new ApplicationException($"Cannot Match MenuItemKey {orderItem.MenuItemKey}, OrderId {viewModel.order.AggregateRootId}");
                }
                orderItem.Price = matchingMenuItem.Price;
            }

            viewModel.order.DeliveryFee = restaurant.Restaurant.DeliveryFee;
            //viewModel.order.SurgePct = restaurant.Restaurant.Surge ? restaurant.Restaurant.SurgePct : 0;
        }

        [HttpPost]
        [Route("api/Order/Payment/Stripe")]
        public async Task<IHttpActionResult> PaymentStripe(StripePaymentVieModel paymentModel)
        {
            var order = await _queryChannel.QueryAsync(new GetOrderByAggregateId() { AggregateId = paymentModel.OrderId });
            var payment = await _stripeService.CreateCharge(paymentModel.PaymentToken, order.OrderTotal, "Huckster Order");
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

            var redirectUrl = await _paypalService.GetRedirectUrl(order.OrderTotal, "Huckster Order", successUrl, failUrl);
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

        public string CompanyName { get; set; }
        public string Instructions { get; set; }
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
