using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Order.Messages;
using Microsoft.Azure.WebJobs;
using RazorEngine;
using RazorEngine.Configuration;
using RazorEngine.Templating;

namespace EmailWebJob
{
    public class EmailMessageHandler
    {
        public static void OrderCompleteMessageHandler([QueueTrigger("ordercomplete")] OrderCompleteMessage message, TextWriter log)
        {
            //var config = new TemplateServiceConfiguration();
            //config.TemplateManager = new ResolvePathTemplateManager(new List<String>() {".\\EmailTemplates"});
            //var service = RazorEngineService.Create(config);
            //Engine.Razor = service;
            //Engine.Razor.AddTemplate("OrderComplete", "OrderComplete.cshtml");
            //string template = "Hello @Model.Name, welcome to RazorEngine!";

            string path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "EmailTemplates", "OrderComplete.cshtml");
            var source = File.ReadAllText(path, System.Text.Encoding.Default);
            Engine.Razor.Compile(source, "OrderComplete");

            path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "EmailTemplates", "Layout.cshtml");
            source = File.ReadAllText(path, System.Text.Encoding.Default);
            Engine.Razor.Compile(source, "Layout.cshtml");

            var result =
                Engine.Razor.Run("OrderComplete", null, new { Name = "World" });
        }
    }
}
