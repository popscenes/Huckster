using Domain.Restaurant;

namespace Domain.Order.Queries.Models
{
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; }
        public DeliverySuburb DeliverySuburb { get; set; }
        public Restaurant.Restaurant Restaurant { get; set; }
        public Customer.Customer Customer { get; set; }
        public DeliverySuburb DeliverAddress { get; set; }
    }
}