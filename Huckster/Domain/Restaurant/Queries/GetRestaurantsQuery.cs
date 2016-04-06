using Domain.Restaurant;
using infrastructure.CQRS;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using infrastructure.DataAccess;
using Newtonsoft.Json;

namespace Domain.Restaurant.Queries
{
    public class GetRestaurantsQuery: IQuery<GetRestaurantsQuery, IEnumerable<Restaurant>>
    {

    }

    public class GetRestaurantsQueryHandler : AdoQueryHandler<GetRestaurantsQuery, IEnumerable<Restaurant>>
    {
        protected override async Task<IEnumerable<Restaurant>> HandleSqlCommandAsync(IDbConnection context, GetRestaurantsQuery argument)
        {
            var restaurants = context.GetList<Restaurant>();
            return restaurants;
        }

        public GetRestaurantsQueryHandler(AdoContext adoContext) : base(adoContext)
        {
        }
    }
}