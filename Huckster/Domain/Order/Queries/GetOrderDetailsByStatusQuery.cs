using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Domain.Order.Queries.Models;
using Domain.Restaurant;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Order.Queries
{
    public class GetOrderDetailsByStatusQuery: IQuery<GetOrderDetailsByStatusQuery, List<OrderDetailsViewModel>>
    {
        public string Status { get; set; }
    }


    public class GetOrderDetailByStatusHandler : AdoQueryHandler<GetOrderDetailsByStatusQuery, List<OrderDetailsViewModel>>
    {
        public GetOrderDetailByStatusHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task<List<OrderDetailsViewModel>> HandleSqlCommandAsync(IDbConnection context, GetOrderDetailsByStatusQuery argument)
        {
            var orders = context.Query<Order>("Select * from [dbo].[Order] where [Status] = @Status",
                new {Status = argument.Status});

            if (!orders.Any())
                return null;
            var orderDetails = orders.Select(_ => OrderQueryHelper.FIllOrderDetails(context, _)).ToList();
            return orderDetails;
        }
    }
}