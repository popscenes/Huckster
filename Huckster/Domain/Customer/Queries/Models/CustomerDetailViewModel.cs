using System.Collections.Generic;
using Domain.Shared;

namespace Domain.Customer.Queries.Models
{
    public class CustomerDetailViewModel
    {
        public Customer Customer { get; set; }
        public List<Address> CustomerAddress { get; set; }
        public List<Order.Order> CustomerOrders { get; set; }
    }
}