using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Order.Messages;
using infrastructure.DataAccess;
using infrastructure.Utility;
using infrastructure.Utility.Infrastructure.Framework;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Ninject;
using RazorEngine;
using RazorEngine.Templating;

namespace EmailWebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var webJobDisabled = ConfigurationManager.AppSettings["WebJobDisabled"];

            if (webJobDisabled.IsNotNullOrWhiteSpace() &&
                webJobDisabled.Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                DiableWebJob();
            }


            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString);

            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference("ordercomplete");
            queue.CreateIfNotExists();
            queue = queueClient.GetQueueReference("orderaccepted");
            queue.CreateIfNotExists();
            queue = queueClient.GetQueueReference("orderdeclined");
            queue.CreateIfNotExists();
            queue = queueClient.GetQueueReference("orderpickup");
            queue.CreateIfNotExists();
            queue = queueClient.GetQueueReference("orderassigned");
            queue.CreateIfNotExists();
            queue = queueClient.GetQueueReference("restaurantaccept");
            queue.CreateIfNotExists();

            CompileRazorView("EmailTemplates", "OrderComplete.cshtml", "OrderComplete");
            CompileRazorView("EmailTemplates", "OrderAccepted.cshtml", "OrderAccepted");
            CompileRazorView("EmailTemplates", "OrderDeclined.cshtml", "OrderDeclined");
            CompileRazorView("EmailTemplates", "OrderPickup.cshtml", "OrderPickUp");
            CompileRazorView("EmailTemplates", "OrderAssigned.cshtml", "OrderAssigned");
            CompileRazorView("EmailTemplates", "Enquiry.cshtml", "Enquiry");
            CompileRazorView("RestaurantAcceptedOrder", "RestaurantAcceptedOrder.cshtml", "Enquiry");
            

            CompileRazorView("EmailTemplates", "Layout.cshtml", "Layout.cshtml");

            InitNinject("Application.", "Infrastructure.", "Domain.");
            NinjectKernel.AppKernel.Bind<AdoContext>().ToMethod(_ => new AdoContext()
            {
                DatabaseName = "BootleggerSql"
            });

            // The following code ensures that the WebJob will be running continuously
            var host = new JobHost();
            host.RunAndBlock();
        }

        private static void DiableWebJob()
        {
            Console.WriteLine("WebJobDisabled.....");
            while (true)
            {
                System.Threading.Thread.Sleep(int.MaxValue);
            }
        }

        protected static void CompileRazorView(string filePath, string filename, string templatename)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, filePath, filename);
            var source = File.ReadAllText(path, System.Text.Encoding.Default);
            Engine.Razor.Compile(source, templatename);
        }


        public static IKernel InitNinject(params String[] defaultAssemblies)
        {
            AllAssemblies.DefaultAssemblyPrefixStrings = defaultAssemblies;
            var assem = AllAssemblies.MatchingDefault()
                .GetOrdered()
                .ThenBy(assembly => assembly
                    .GetName().Name
                    .ToLower().Contains("infrastructure")
                    ? 0
                    : 1)
                .ToList();

            Ninject.IKernel kernel = NinjectKernel.CreateKernel(assem);
            return kernel;
        }
    }
}
