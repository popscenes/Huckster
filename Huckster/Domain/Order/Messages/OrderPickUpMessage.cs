using System;
using infrastructure.Messaging;

namespace Domain.Order.Messages
{
    public class OrderPickUpMessage : IMessage
    {
        public string QueueName 
        {
            get { return "orderpickup"; }
            set { }
        }
        public Guid OrderAggregateRootId { get; set; }
    }
}