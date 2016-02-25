using System;
using infrastructure.Messaging;

namespace Domain.Order.Messages
{
    public class OrderAcceptedMessage : IMessage
    {
        public string QueueName 
        {
            get { return "orderaccepted"; }
            set { }
        }
        public Guid OrderAggregateRootId { get; set; }
    }
}