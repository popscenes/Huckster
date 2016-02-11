using System;
using System.Data;
using System.Threading.Tasks;
using DapperExtensions;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Order.Commands
{
    public class SetOrderStatusCommand: ICommand
    {
        public int OrderId { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }

    public class SetOrderStatusCommandHandler : AdoCommandHandler<SetOrderStatusCommand>
    {
        public SetOrderStatusCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task HandleSqlCommandAsync(IDbConnection context, SetOrderStatusCommand command)
        {
            Order order = context.Get<Order>(command.OrderId);
            order.Status = command.OrderStatus.ToString();
            order.LastModifiedDateTime = DateTime.UtcNow;
            context.Update(order);
        }
    }
}