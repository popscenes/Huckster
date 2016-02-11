using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Customer.Queries
{
    public class GetCustomerByEmailorMobile : IQuery<GetCustomerByEmailorMobile, Customer>
    {
        public string Email { get; set; }
        public string Mobile { get; set; }
    }

    public class GetCustomerByEmailorMobileHandler : AdoQueryHandler<GetCustomerByEmailorMobile, Customer>
    {

        protected async override Task<Customer> HandleSqlCommandAsync(IDbConnection context, GetCustomerByEmailorMobile argument)
        {
            var customer = context.Query<Customer>("Select * FROM [dbo].[Customer] where Mobile = @Mobile or Email = @Email", new { Mobile = argument.Mobile, Email = argument.Email }).FirstOrDefault();
            return customer;
        }

        public GetCustomerByEmailorMobileHandler(AdoContext adoContext) : base(adoContext)
        {
        }
    }
}