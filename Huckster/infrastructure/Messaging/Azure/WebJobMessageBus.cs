using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Newtonsoft.Json;

namespace infrastructure.Messaging.Azure
{
    public class WebJobMessageBus: IMessageBus
    {
        public void SendMessage(IMessage message)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString);
            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(message.QueueName);
            var queueMessage = new CloudQueueMessage(JsonConvert.SerializeObject(message));
            queue.AddMessage(queueMessage, null, null, null, null);
        }
    }
}
