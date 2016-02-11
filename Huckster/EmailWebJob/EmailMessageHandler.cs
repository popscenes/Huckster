using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Domain.Order.Messages;
using Domain.Order.Queries;
using infrastructure.CQRS;
using Microsoft.Azure.WebJobs;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;
using SendGrid;

namespace EmailWebJob
{
    public class EmailMessageHandler
    {
        public async static void OrderCompleteMessageHandler([QueueTrigger("ordercomplete")] OrderCompleteMessage message, TextWriter log)
        {
            //var config = new TemplateServiceConfiguration();
            //config.TemplateManager = new ResolvePathTemplateManager(new List<String>() {".\\EmailTemplates"});
            //var service = RazorEngineService.Create(config);
            //Engine.Razor = service;
            //Engine.Razor.AddTemplate("OrderComplete", "OrderComplete.cshtml");
            //string template = "Hello @Model.Name, welcome to RazorEngine!";



            var queryChannel = new SimpleQueryChannel();
            var order =  await queryChannel.QueryAsync(new GetOrderDetailByAggregateId() {AggregateId = message.OrderAggregateRootId});

            var result =
                Engine.Razor.Run("OrderComplete", null, new
                {
                    Time = order.Order.DeliveryTime.ToString(),
                    Restaurant = order.Restaurant.Name,
                    Address = ""
                });

            var mail = new SendGridMessage {From = new MailAddress("no-reply@huckster.com.aum")};
            mail.AddTo(order.Order.CustomerEmail);
            mail.Subject = "Huckster Order Complete";
            mail.Html = result;
            var credentials = new NetworkCredential("azure_02d8849bebc2a0f9cddbbc630249d6c6@azure.com", "0g888JHePRKZ5dN");
            var transportWeb = new Web(credentials);
            await transportWeb.DeliverAsync(mail);
        }
    }
}
