using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using infrastructure.Messaging;

namespace Domain.Order.Messages
{
    public class OrderCompleteMessage: IMessage
    {
        public Guid OrderAggregateRootId { get; set; }

        public string QueueName {
            get { return "ordercomplete"; }
            set { }
        }
    }
}
