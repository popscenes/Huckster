using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using infrastructure.Messaging;

namespace Domain.Order.Messages
{
    public class OrderAssignedMessage: IMessage
    {
        public string QueueName
        {
            get { return "orderassigned"; }
            set { }
        }
        public Guid OrderAggregateRootId { get; set; }

        public string AssignedUserId { get; set; }
    }
}
