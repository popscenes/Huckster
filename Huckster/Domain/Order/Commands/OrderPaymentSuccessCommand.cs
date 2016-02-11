using System;
using System.Data;
using System.Threading.Tasks;
using DapperExtensions;
using Domain.Payment;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Order.Commands
{
    public class OrderPaymentSuccessCommand : ICommand
    {
        public Order Order { get; set; }
        public PaymentEvent Payment { get; set; }
    }

    public class OrderPaymentSuccessCommandHandler : AdoCommandHandler<OrderPaymentSuccessCommand>
    {
        public OrderPaymentSuccessCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task HandleSqlCommandAsync(IDbConnection context, OrderPaymentSuccessCommand command)
        {
            Order order = context.Get<Order>(command.Order.Id);
            order.Status = OrderStatus.PaymentSucccessful.ToString();
            order.LastModifiedDateTime = DateTime.UtcNow;
            context.Update(order);

            command.Payment.ParentAggregateId = order.AggregateRootId;
            context.Insert(command.Payment);
        }
    }
}