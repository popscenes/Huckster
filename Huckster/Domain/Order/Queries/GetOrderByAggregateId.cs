using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using Domain.Restaurant;
using Domain.Shared;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Order.Queries
{
    public class GetOrderByAggregateId : IQuery<GetOrderByAggregateId, Order>
    {
        public Guid AggregateId { get; set; }
    }

    public class GetOrderByAggregateIdQueryHandler : AdoQueryHandler<GetOrderByAggregateId, Order>
    {
        public GetOrderByAggregateIdQueryHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task<Order> HandleSqlCommandAsync(IDbConnection context, GetOrderByAggregateId argument)
        {
            var order =
                context.Query<Order>("Select * from [dbo].[Order] where [AggregateRootId] = @AggregateRootId",
                    new {AggregateRootId = argument.AggregateId}).FirstOrDefault();
            order.OrderItems =
                context.Query<OrderItem>("Select * from [dbo].[OrderItem] where ParentAggregateId = @ParentAggregateId",
                    new {ParentAggregateId = argument.AggregateId}).ToList();
            return order;
        }
    }
}