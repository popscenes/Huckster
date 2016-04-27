using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Domain.Enquiry.Messages;
using Domain.Order.Messages;
using Domain.Order.Queries;
using Domain.Shared;
using infrastructure.CQRS;
using Microsoft.Azure.WebJobs;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using SendGrid;
using NLog;

namespace EmailWebJob
{
    public class EmailMessageHandler
    {
        private static Logger logger = LogManager.GetLogger("mail");
        public static async void OrderAssignedMessageHandler([QueueTrigger("orderassigned")] OrderAssignedMessage message, TextWriter log)
        {
            try
            {
                var queryChannel = new SimpleQueryChannel();
                var order = await queryChannel.QueryAsync(new GetOrderAdminDetailByAggregateId() { AggregateId = message.OrderAggregateRootId });
                var assignedUser = order.DeliveryUser;

                var adminUrl = ConfigurationManager.AppSettings["AdminUrl"];

                var result =
                    Engine.Razor.Run("OrderAssigned", null, new
                    {
                        Time = order.Order.DeliveryTime.ToString(),
                        Restaurant = order.Restaurant.Name,
                        RestaurantAddress = order.RestaurantAddress.ToString(),
                        Address = order.DeliverAddress.ToString(),
                        PickupTime = order.Order.PickUpTime.ToString(),
                        OrderUrl = $"{adminUrl}Orders/Detail?orderId={order.Order.AggregateRootId}"
                    });

                await SendGridEmail(assignedUser.Email, "Assigned Delivery", result);
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e.ToString() + "\n" + e.StackTrace.ToString());
                throw;
            }
            
        }

        public static async void OrderDeclinedMessageHandler([QueueTrigger("orderdeclined")] OrderDeclinedMessage message, TextWriter log)
        {
            try
            {
                var queryChannel = new SimpleQueryChannel();
                var order = await queryChannel.QueryAsync(new GetOrderDetailByAggregateId() { AggregateId = message.OrderAggregateRootId });

                var result =
                    Engine.Razor.Run("OrderDeclined", null, new
                    {
                        Time = order.Order.DeliveryTime.ToString(),
                        Restaurant = order.Restaurant.Name,
                        Address = ""
                    });

                await SendGridEmail(order.Customer.Email, "Your Huckster Order - Declined", result);
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e.ToString() + "\n" + e.StackTrace.ToString());
                throw;
            }
            
        }

        public static async void OrderPickUpMessageHandler([QueueTrigger("orderpickup")] OrderPickUpMessage message, TextWriter log)
        {
            try
            {
                var queryChannel = new SimpleQueryChannel();
                var order = await queryChannel.QueryAsync(new GetOrderDetailByAggregateId() { AggregateId = message.OrderAggregateRootId });

                var result =
                    Engine.Razor.Run("OrderPickUp", null, new
                    {
                        Time = order.Order.DeliveryTime.ToString(),
                        Restaurant = order.Restaurant.Name,
                        Address = ""
                    });

                await SendGridEmail(order.Customer.Email, "Your Huckster Order - Picked Up", result);
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e.ToString() + "\n" + e.StackTrace.ToString());
                throw;
            }
            
        }

        public static async void OrderAcceptedMessageHandler([QueueTrigger("orderaccepted")] OrderAcceptedMessage message, TextWriter log)
        {
            try
            {
                var queryChannel = new SimpleQueryChannel();
                var order = await queryChannel.QueryAsync(new GetOrderDetailByAggregateId() { AggregateId = message.OrderAggregateRootId });

                var result =
                    Engine.Razor.Run("OrderAccepted", null, new
                    {
                        Time = order.Order.DeliveryTime.ToString(),
                        Restaurant = order.Restaurant.Name,
                        Address = ""
                    });

                await SendGridEmail(order.Customer.Email, "Your Huckster Order - Accepted", result);
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e.ToString() + "\n" + e.StackTrace.ToString());
                throw;
            }
            
        }

        public static async Task OrderCompleteMessageHandler([QueueTrigger("ordercomplete")] OrderCompleteMessage message, TextWriter log)
        {
            try
            {
                var queryChannel = new SimpleQueryChannel();
                var order = await queryChannel.QueryAsync(new GetOrderDetailByAggregateId() { AggregateId = message.OrderAggregateRootId });

                var result =
                    Engine.Razor.Run("OrderComplete", null, new
                    {
                        Time = order.Order.DeliveryTime.ToString(),
                        Restaurant = order.Restaurant.Name,
                        Address = order.DeliverAddress.ToString(),
                        OrderId = order.Order.Id,
                        Mobile = order.Order.CustomerMobile
                    });

                await SendGridEmail(order.Customer.Email, "Your Huckster Order", result);
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e.ToString() + "\n" + e.StackTrace.ToString());
                throw;
            }
            
        }

        public static async Task EnquiryMessageHandler([QueueTrigger("emailenquirymessage")] EmailEnquiryMessage message, TextWriter log)
        {
            try
            {
                var result = Engine.Razor.Run("Enquiry", null, message.Enquiry);
                var emailaddress = ConfigurationManager.AppSettings["EnquiryEmailTo"];
                await SendGridEmail(emailaddress, $"New Enquiry - {message.Enquiry.Subject}", result);
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e.ToString() + "\n" + e.StackTrace.ToString());
                throw;
            }
            
        }

        public static async Task RestaurantOrderAcceptMessageHandler([QueueTrigger("restaurantaccept")] RestaurantOrderAcceptMessage message, TextWriter log)
        {
            try
            {
                var queryChannel = new SimpleQueryChannel();
                var order = await queryChannel.QueryAsync(new GetOrderDetailByAggregateId() { AggregateId = message.OrderAggregateRootId });

                var emailaddress = ConfigurationManager.AppSettings["EnquiryEmailTo"];
                var adminUrl = ConfigurationManager.AppSettings["AdminUrl"];

                var result =
                    Engine.Razor.Run("RestaurantAcceptedOrder", null, new
                    {
                        Time = order.Order.DeliveryTime.ToString(),
                        Restaurant = order.Restaurant.Name,
                        RestaurantAddress = order.RestaurantAddress.ToString(),
                        Address = order.DeliverAddress.ToString(),
                        PickupTime = order.Order.PickUpTime.ToString(),
                        OrderUrl = $"{adminUrl}Orders/Detail?orderId={order.Order.AggregateRootId}"
                    });

                await SendGridEmail(emailaddress, "Order Accepted By Restaurant", result);
            }
            catch (Exception e)
            {
                logger.Log(LogLevel.Error, e.ToString() + "\n" + e.StackTrace.ToString());
                throw;
            }
            
        }

        protected static async Task SendGridEmail(string to, string subject, string body)
        {
            var mail = new SendGridMessage { From = new MailAddress("no-reply@huckster.com.aum") };
            mail.AddTo(to);
            mail.Subject = subject;
            mail.Html = body;
            var credentials = new NetworkCredential("azure_02d8849bebc2a0f9cddbbc630249d6c6@azure.com", "0g888JHePRKZ5dN");
            var transportWeb = new Web(credentials);
            await transportWeb.DeliverAsync(mail);
        }
    }
}
