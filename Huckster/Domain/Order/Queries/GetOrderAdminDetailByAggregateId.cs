using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Domain.Order.Queries.Models;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Order.Queries
{
    public class GetOrderAdminDetailByAggregateId : IQuery<GetOrderAdminDetailByAggregateId, OrderAdminDetailsViewModel>
    {
        public Guid AggregateId { get; set; }
    }

    public class GetOrderAdminDetailByAggregateIdHandler : AdoQueryHandler<GetOrderAdminDetailByAggregateId, OrderAdminDetailsViewModel>
    {
        public GetOrderAdminDetailByAggregateIdHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task<OrderAdminDetailsViewModel> HandleSqlCommandAsync(IDbConnection context, GetOrderAdminDetailByAggregateId argument)
        {
            var order =
                context.Query<Order>("Select * from [dbo].[Order] where [AggregateRootId] = @AggregateRootId",
                    new { AggregateRootId = argument.AggregateId }).FirstOrDefault();

            var orderDetails = OrderQueryHelper.FIllAdminOrderDetails(context, order);
            return orderDetails;
        }
    }
}