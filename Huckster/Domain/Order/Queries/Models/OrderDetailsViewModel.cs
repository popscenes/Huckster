using Domain.Restaurant;
using Domain.Shared;

namespace Domain.Order.Queries.Models
{
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; }
        public DeliverySuburb DeliverySuburb { get; set; }
        public Restaurant.Restaurant Restaurant { get; set; }
        public Customer.Customer Customer { get; set; }
        public Address DeliverAddress { get; set; }
        public Address RestaurantAddress { get; set; }
    }
}