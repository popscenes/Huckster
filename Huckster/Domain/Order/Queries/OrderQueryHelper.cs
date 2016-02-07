using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Domain.Order.Queries.Models;
using Domain.Restaurant;
using Domain.Shared;

namespace Domain.Order.Queries
{
    public static class OrderQueryHelper
    {

        public static OrderDetailsViewModel FIllOrderDetails(IDbConnection context, Order order)
        {
            order.OrderItems = context.Query<OrderItem>("Select * from [dbo].[OrderItem] where ParentAggregateId = @ParentAggregateId",
                    new { ParentAggregateId = order.AggregateRootId }).ToList();

            var deliverySuburb = context.Query<DeliverySuburb>("Select * from [dbo].[DeliverySuburb] where Id = @DeliverySuburb",
                    new { DeliverySuburb = order.DeliverySuburbId }).FirstOrDefault();

            var deliverAddress = context.Query<Address>("Select * from [dbo].[Address] where ParentAggregateId = @ParentAggregateId",
                    new { ParentAggregateId = order.AggregateRootId }).FirstOrDefault();

            var restaurant = context.Query<Restaurant.Restaurant>("Select * from [dbo].[Restaurant] where AggregateRootId = @AggregateRootId",
                    new { AggregateRootId = order.RestaurantId }).FirstOrDefault();

            var restaurantAddress = context.Query<Address>("Select * from [dbo].[Address] where ParentAggregateId = @ParentAggregateId",
                    new { ParentAggregateId = restaurant.AggregateRootId }).FirstOrDefault();

            var customer = context.Query<Customer.Customer>("Select * from [dbo].[Customer] where AggregateRootId = @AggregateRootId",
                    new { AggregateRootId = order.CustomerId }).FirstOrDefault();

            return new OrderDetailsViewModel()
            {
                Order = order,
                DeliverySuburb = deliverySuburb,
                DeliverAddress = deliverAddress,
                Restaurant = restaurant,
                Customer = customer,
                RestaurantAddress = restaurantAddress
            };
        }
    }
}
