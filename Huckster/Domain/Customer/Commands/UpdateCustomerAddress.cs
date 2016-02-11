using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using Domain.Shared;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Customer.Commands
{
    public class UpdateCustomerAddress: ICommand
    {
        public Customer Customer { get; set; }
        public Address Address { get; set; }
    }

    public class UpdateCustomerAndAddressHandler : AdoCommandHandler<UpdateCustomerAddress>
    {
        public UpdateCustomerAndAddressHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task HandleSqlCommandAsync(IDbConnection context, UpdateCustomerAddress command)
        {
            var existingAddresses = context.Query<Address>("Select * from [dbo].[Address] where ParentAggregateId = @ParentAggregateId",
                new { ParentAggregateId = command.Customer.AggregateRootId }).ToList();

            bool found = existingAddresses.Any(existingAddress => existingAddress.Equals(command.Address));

            if (found)
            {
                return;
            }

            var newAddress = command.Address;
            newAddress.ParentAggregateId = command.Customer.AggregateRootId;
            context.Insert(newAddress);
        }
    }
}