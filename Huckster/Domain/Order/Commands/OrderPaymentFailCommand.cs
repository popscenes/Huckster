using System;
using System.Data;
using System.Threading.Tasks;
using DapperExtensions;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Order.Commands
{
    public class OrderPaymentFailCommand: ICommand
    {
        public Order Order { get; set; }
    }

    public class OrderPaymentFailCommandHandler : AdoCommandHandler<OrderPaymentFailCommand>
    {
        public OrderPaymentFailCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task HandleSqlCommandAsync(IDbConnection context, OrderPaymentFailCommand command)
        {
            Order order = context.Get<Order>(command.Order.Id);
            order.Status = OrderStatus.PaymentFailed.ToString();
            order.LastModifiedDateTime = DateTime.UtcNow;
            context.Update(order);
        }
    }
}