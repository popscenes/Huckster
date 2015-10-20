using System.Data;
using System.Threading.Tasks;
using Domain.Shared;
using infrastructure.CQRS;
using infrastructure.DataAccess;
using Dapper;
using DapperExtensions;

namespace Domain.Order.Commands
{
    public class UpdateOrderCustomerAndAddress: ICommand
    {
        public int OrderId { get; set; }
        public Customer.Customer Customer { get; set; }
        public Address Address { get; set; }
    }

    public class UpdateOrderCustomerAndAddressHandler : AdoCommandHandler<UpdateOrderCustomerAndAddress>
    {
        public UpdateOrderCustomerAndAddressHandler(AdoContext adoContext) : base(adoContext)
        {
            
        }

        protected async override Task HandleSqlCommandAsync(IDbConnection context, UpdateOrderCustomerAndAddress command)
        {
            var order = context.Get<Order>(command.OrderId);
            order.CustomerId = command.Customer.AggregateRootId;
            order.CustomerEmail = command.Customer.Email;
            order.CustomerMobile = command.Customer.Mobile;
            context.Update(order);

            var address = command.Address;
            address.ParentAggregateId = order.AggregateRootId;
            context.Insert(address);

        }
    }
}