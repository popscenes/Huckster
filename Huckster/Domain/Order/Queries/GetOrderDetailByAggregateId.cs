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

            var orderDetails = OrderQueryHelper.FIllOrderDetails(context, order);
            return orderDetails;
        }
    }
}
