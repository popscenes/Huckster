using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using Domain.Shared;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Order.Commands
{
    public class UpdateOrderAddressCommand : ICommand
    {
        public int OrderId { get; set; }
        public Address Address { get; set; }

        public class UpdateOrderAddressCommandHandler : AdoCommandHandler<UpdateOrderAddressCommand>
        {
            public UpdateOrderAddressCommandHandler(AdoContext adoContext) : base(adoContext)
            {
            }

            protected async override Task HandleSqlCommandAsync(IDbConnection context, UpdateOrderAddressCommand command)
            {
                var existingAddress = context.Query<Address>("Select * from [dbo].[Address] where ParentAggregateId = @ParentAggregateId", new { ParentAggregateId = command.OrderId }).ToList();

                foreach (var oldAddress in existingAddress)
                {
                    context.Delete(oldAddress);
                }

                var order = context.Get<Order>(command.OrderId);
                var address = command.Address;
                address.ParentAggregateId = order.AggregateRootId;
                context.Insert(address);
            }
        }
    }
}