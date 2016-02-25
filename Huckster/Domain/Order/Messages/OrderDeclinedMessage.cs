using System;
using infrastructure.Messaging;

namespace Domain.Order.Messages
{
    public class OrderDeclinedMessage : IMessage
    {
        public string QueueName 
        {
            get { return "orderdeclined"; }
            set { }
        }
        public Guid OrderAggregateRootId { get; set; }
    }
}