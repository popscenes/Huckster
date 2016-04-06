using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Restaurant.Queries
{
    public class GetRestaurantAccessForUserQuery: IQuery<GetRestaurantAccessForUserQuery, List<RestaurantAccess>>
    {
        public string UserId { get; set; }
    }

    public class GetRestaurantAccessForUserQueryHandler : AdoQueryHandler<GetRestaurantAccessForUserQuery, List<RestaurantAccess>>
    {
        protected override async Task<List<RestaurantAccess>> HandleSqlCommandAsync(IDbConnection context, GetRestaurantAccessForUserQuery argument)
        {
            var restaurantAccess = context.Query<RestaurantAccess>("SELECT ra.UserId, ra.RestaurantAggrgateRootId, r.Name ,[UserId],[RestaurantAggrgateRootId] FROM [dbo].[RestaurantAccess] ra inner join Restaurant r on r.AggregateRootId = ra.RestaurantAggrgateRootId where ra.UserId = @UserId", new {UserId = argument.UserId}).ToList();
            return restaurantAccess;
        }

        public GetRestaurantAccessForUserQueryHandler(AdoContext adoContext) : base(adoContext)
        {
        }
    }
}