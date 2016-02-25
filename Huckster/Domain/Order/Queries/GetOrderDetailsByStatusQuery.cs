using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Domain.Order.Queries.Models;
using Domain.Restaurant;
using infrastructure.CQRS;
using infrastructure.DataAccess;
using infrastructure.Utility;

namespace Domain.Order.Queries
{
    public class GetOrderDetailsByStatusQuery: IQuery<GetOrderDetailsByStatusQuery, List<OrderAdminDetailsViewModel>>
    {
        public string Status { get; set; }
        public string DeliveryUserId { get; set; }
    }


    public class GetOrderDetailByStatusHandler : AdoQueryHandler<GetOrderDetailsByStatusQuery, List<OrderAdminDetailsViewModel>>
    {
        public GetOrderDetailByStatusHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task<List<OrderAdminDetailsViewModel>> HandleSqlCommandAsync(IDbConnection context, GetOrderDetailsByStatusQuery argument)
        {
            var orders = context.Query<Order>("Select * from [dbo].[Order] where [Status] = @Status",
                new {Status = argument.Status});

            if (argument.DeliveryUserId.IsNotNullOrWhiteSpace())
            {
                orders = orders.Where(_ => _.DeliveryUserId != null && _.DeliveryUserId.Equals(argument.DeliveryUserId));
            }

            if (!orders.Any())
                return null;
            var orderDetails = orders.Select(_ => OrderQueryHelper.FIllAdminOrderDetails(context, _)).ToList();
            return orderDetails;
        }
    }
}