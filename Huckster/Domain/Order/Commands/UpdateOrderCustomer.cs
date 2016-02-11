using System.Data;
using System.Threading.Tasks;
using DapperExtensions;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Order.Commands
{
    public class UpdateOrderCustomer: ICommand
    {
        public Customer.Customer Customer { get; set; }
        public Order Order { get; set; }
    }

    public class UpdateOrderCustomersHandler : AdoCommandHandler<UpdateOrderCustomer>
    {
        public UpdateOrderCustomersHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task HandleSqlCommandAsync(IDbConnection context, UpdateOrderCustomer command)
        {
            var order = context.Get<Order>(command.Order.Id);
            order.CustomerId = command.Customer.AggregateRootId;
            order.CustomerEmail = command.Customer.Email;
            order.CustomerMobile = command.Customer.Mobile;
            context.Update(order);
        }
    }
}