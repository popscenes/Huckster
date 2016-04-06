using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Restaurant.Queries
{
    public class GetRestaurantAccessUsersQuery: IQuery<GetRestaurantAccessUsersQuery,List<RestaurantAccess>>
    {
    }

    public class GetRestaurantAccessUsersQueryHandler : AdoQueryHandler<GetRestaurantAccessUsersQuery, List<RestaurantAccess>>
    {
        protected override async Task<List<RestaurantAccess>> HandleSqlCommandAsync(IDbConnection context, GetRestaurantAccessUsersQuery argument)
        {
            var restaurantAccess = context.Query<RestaurantAccess>("SELECT ra.UserId, ra.RestaurantAggrgateRootId, r.Name ,[UserId],[RestaurantAggrgateRootId] FROM [dbo].[RestaurantAccess] ra inner join Restaurant r on r.AggregateRootId = ra.RestaurantAggrgateRootId").ToList();
            return restaurantAccess;
        }

        public GetRestaurantAccessUsersQueryHandler(AdoContext adoContext) : base(adoContext)
        {
        }
    }
}