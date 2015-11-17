using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Domain.Order.Queries.Models;
using Domain.Restaurant;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Order.Queries
{
    public class GetOrderDetailByAggregateId : IQuery<GetOrderDetailByAggregateId, OrderDetailsViewModel>
    {
        public Guid AggregateId { get; set; }
    }

    public class GetOrderDetailByAggregateIdHandler : AdoQueryHandler<GetOrderDetailByAggregateId, OrderDetailsViewModel>
    {
        public GetOrderDetailByAggregateIdHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task<OrderDetailsViewModel> HandleSqlCommandAsync(IDbConnection context, GetOrderDetailByAggregateId argument)
        {
            var order =
                context.Query<Order>("Select * from [dbo].[Order] where [AggregateRootId] = @AggregateRootId",
                    new { AggregateRootId = argument.AggregateId }).FirstOrDefault();
            order.OrderItems =
                context.Query<OrderItem>("Select * from [dbo].[OrderItem] where ParentAggregateId = @ParentAggregateId",
                    new { ParentAggregateId = argument.AggregateId }).ToList();

            var deliverySuburb = context.Query<DeliverySuburb>("Select * from [dbo].[DeliverySuburb] where Id = @DeliverySuburb",
                    new { DeliverySuburb = order.DeliverySuburbId }).FirstOrDefault();

            var restaurant = context.Query<Restaurant.Restaurant>("Select * from [dbo].[Restaurant] where AggregateRootId = @AggregateRootId",
                    new { AggregateRootId = order.RestaurantId }).FirstOrDefault();

            return new OrderDetailsViewModel()
            {
                Order = order,
                DeliverySuburb = deliverySuburb,
                Restaurant =  restaurant
            };
        }
    }
}
