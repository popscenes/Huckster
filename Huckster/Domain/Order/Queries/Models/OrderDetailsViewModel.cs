using Domain.Restaurant;

namespace Domain.Order.Queries.Models
{
    public class OrderDetailsViewModel
    {
        public Order Order { get; set; }
        public DeliverySuburb DeliverySuburb { get; set; }
    }
}