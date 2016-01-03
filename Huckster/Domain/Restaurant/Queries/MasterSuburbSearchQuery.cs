using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Restaurant.Queries
{
    public class MasterSuburbSearchQuery : IQuery<MasterSuburbSearchQuery, List<DeliverySuburb>>
    {
        public string Searchtext { get; set; }
    }

    public class MasterSuburbSearchQueryHandler : AdoQueryHandler<MasterSuburbSearchQuery, List<DeliverySuburb>>
    {
        public MasterSuburbSearchQueryHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task<List<DeliverySuburb>> HandleSqlCommandAsync(IDbConnection context, MasterSuburbSearchQuery argument)
        {
            var suburbs = context.Query<DeliverySuburb>("Select * from [dbo].[MasterSuburb] where Suburb like @SearchText", new { SearchText = argument.Searchtext + "%" }).ToList();
            return suburbs;
        }
    }
}