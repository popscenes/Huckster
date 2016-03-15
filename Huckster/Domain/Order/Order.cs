using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
using Domain.Shared;
using infrastructure.Domain;

namespace Domain.Order
{
    public enum OrderStatus
    {
        Placed,
        PaymentSucccessful,
        PaymentFailed,
        RestaurantAccepted,
        Cancelled,
        PickedUp,
        Delivered,
        Archived
    }
    public class Order: IEntity
    {
        public Guid RestaurantId { get; set; }
        public Guid CustomerId { get; set; }
        public int DeliverySuburbId { get; set; }
        public DateTime DeliveryTime { get; set; }

        public string CustomerMobile { get; set; }
        public string CustomerEmail { get; set; }

        public string CompanyName { get; set; }
        public string Instructions { get; set; }

        public DateTime CreateDateTime { get; set; }
        public DateTime LastModifiedDateTime { get; set; }
        public int Id { get; set; }

        public DateTime? PickUpTime { get; set; }
        public String DeliveryUserId { get; set; }

        public Decimal DeliveryFee { get; set; }

        public Guid AggregateRootId { get; set; }
        public String Status { get; set; }
        //public Address Address { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public Decimal OrderTotal {
            get {
                if (OrderItems == null)
                {
                    return 0.0m;
                }
                return OrderItems.Sum(_ => _.Price*_.Quantity) + DeliveryFee;
            }
        }
    }

    public class DeliveryUser
    {
        public string Id { get; set; }
        public String Email { get; set; }
        public String UserName { get; set; }
    }

    public class OrderItem : IValueObject
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public String Notes { get; set; }
        public int Id { get; set; }
        public Guid ParentAggregateId { get; set; }
        public int MenuItemKey { get; set; }
    }

    public class PersonMapper : ClassMapper<Order>
    {
        public PersonMapper()
        {
            Table("Order");
            //Map(m => m.Address).Ignore();
            Map(m => m.OrderItems).Ignore();
            Map(m => m.OrderTotal).Ignore();
            
            AutoMap();
        }
    }
}
