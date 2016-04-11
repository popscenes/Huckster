using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using infrastructure.Messaging;

namespace Domain.Order.Messages
{
    public class RestaurantOrderAcceptMessage: IMessage
    {
        public string QueueName
        {
            get { return "restaurantaccept"; }
            set { }
        }
        public Guid OrderAggregateRootId { get; set; }
    }
}
