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
    public class GetOrderDetailsByUserStatusQuery: IQuery<GetOrderDetailsByUserStatusQuery, List<OrderAdminDetailsViewModel>>
    {
        public string Status { get; set; }
        public string UserId { get; set; }
    }


    public class GetOrderDetailsByUserStatusQuerysHandler : AdoQueryHandler<GetOrderDetailsByUserStatusQuery, List<OrderAdminDetailsViewModel>>
    {
        public GetOrderDetailsByUserStatusQuerysHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task<List<OrderAdminDetailsViewModel>> HandleSqlCommandAsync(IDbConnection context, GetOrderDetailsByUserStatusQuery argument)
        {
            var restaurantAccess = context.Query<RestaurantAccess>("Select * from[dbo].[RestaurantAccess] where [UserId] = @UserId", new { UserId = argument.UserId }).FirstOrDefault();

            var orders = context.Query<Order>("Select * from [dbo].[Order] where [Status] = @Status and RestaurantId = @RestaurantId",
                new {Status = argument.Status, RestaurantId = restaurantAccess.RestaurantAggrgateRootId});

            if (!orders.Any())
                return null;
            var orderDetails = orders.Select(_ => OrderQueryHelper.FIllAdminOrderDetails(context, _)).ToList();
            return orderDetails;
        }
    }
}