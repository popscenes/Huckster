using System;
using System.Data;
using System.Threading.Tasks;
using DapperExtensions;
using infrastructure.CQRS;
using infrastructure.DataAccess;
using Omu.ValueInjecter;

namespace Domain.Order.Commands
{
    public class UpdateOrderCommand: ICommand
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }

    public class UpdateOrderCommandHandler : AdoCommandHandler<UpdateOrderCommand>
    {
        public UpdateOrderCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task HandleSqlCommandAsync(IDbConnection context, UpdateOrderCommand command)
        {
            Order order = context.Get<Order>(command.OrderId);
            order.InjectFrom(command.Order);
            order.LastModifiedDateTime = DateTime.UtcNow;
            context.Update(order);
        }
    }
}