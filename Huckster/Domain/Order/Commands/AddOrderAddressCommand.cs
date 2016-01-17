using System.Data;
using System.Threading.Tasks;
using DapperExtensions;
using Domain.Shared;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Order.Commands
{
    public class AddOrderAddressCommand : ICommand
    {
        public int OrderId { get; set; }
        public Address Address { get; set; }

        public class AddOrderAddressCommandHandler : AdoCommandHandler<AddOrderAddressCommand>
        {
            public AddOrderAddressCommandHandler(AdoContext adoContext) : base(adoContext)
            {
            }

            protected async override Task HandleSqlCommandAsync(IDbConnection context, AddOrderAddressCommand command)
            {
                var order = context.Get<Order>(command.OrderId);
                var address = command.Address;
                address.ParentAggregateId = order.AggregateRootId;
                context.Insert(address);
            }
        }
    }
}