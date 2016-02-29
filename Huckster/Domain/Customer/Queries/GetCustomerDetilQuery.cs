using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Domain.Customer.Queries.Models;
using Domain.Shared;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Customer.Queries
{
    public class GetCustomerDetilQuery: IQuery<GetCustomerDetilQuery, CustomerDetailViewModel>
    {
        public Guid AggregateRootId { get; set; }
    }

    public class GetCustomerDetilQueryHandler : AdoQueryHandler<GetCustomerDetilQuery, CustomerDetailViewModel>
    {
        public GetCustomerDetilQueryHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task<CustomerDetailViewModel> HandleSqlCommandAsync(IDbConnection context, GetCustomerDetilQuery argument)
        {
            var customer =
                context.Query<Customer>("Select * from [dbo].[Customer] where [AggregateRootId] = @AggregateRootId",
                    new { AggregateRootId = argument.AggregateRootId }).FirstOrDefault();

            var address = context.Query<Address>("Select * from [dbo].[Address] where ParentAggregateId = @ParentAggregateId",
                    new { ParentAggregateId = argument.AggregateRootId }).ToList();

            var orders = context.Query<Order.Order>("Select * from [dbo].[Order] where CustomerId = @ParentAggregateId",
                    new { ParentAggregateId = argument.AggregateRootId }).ToList();

            return new CustomerDetailViewModel()
            {
                Customer = customer,
                CustomerAddress = address,
                CustomerOrders = orders
            };
        }
    }
}