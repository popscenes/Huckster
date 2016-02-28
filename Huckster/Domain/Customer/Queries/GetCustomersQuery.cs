using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using infrastructure.CQRS;
using infrastructure.DataAccess;
using infrastructure.Utility;

namespace Domain.Customer.Queries
{
    public class GetCustomersQuery: IQuery<GetCustomersQuery, List<Customer>>
    {
        public string Searchtext { get; set; }
    }

    public class GetCustomersQueryHandler : AdoQueryHandler<GetCustomersQuery, List<Customer>>
    {
        public GetCustomersQueryHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task<List<Customer>> HandleSqlCommandAsync(IDbConnection context, GetCustomersQuery argument)
        {
            if (argument.Searchtext.IsNotNullOrWhiteSpace())
            {
                return context.Query<Customer>("Select * FROM [dbo].[Customer] " +
                                                       "where Mobile like @SearchText or Email like @SearchText or Name like @SearchText",
                    new {SearchText = argument.Searchtext + '%'}).ToList();
            }

            return context.Query<Customer>("Select * FROM [dbo].[Customer] ",new { SearchText = argument.Searchtext + '%' }).ToList();
        }
    }
}