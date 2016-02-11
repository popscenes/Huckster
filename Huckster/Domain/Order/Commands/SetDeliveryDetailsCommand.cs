using System;
using System.Data;
using System.Threading.Tasks;
using DapperExtensions;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Order.Commands
{
    public class SetDeliveryDetailsCommand : ICommand
    {
        public DateTime PickUpDateTime { get; set; }
        public string DeliveryUserId { get; set; }
        public object OrderId { get; set; }
    }

    public class SetDeliveryDetailsCommandHandler : AdoCommandHandler<SetDeliveryDetailsCommand>
    {
        public SetDeliveryDetailsCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task HandleSqlCommandAsync(IDbConnection context, SetDeliveryDetailsCommand command)
        {
            Order order = context.Get<Order>(command.OrderId);
            order.DeliveryUserId = command.DeliveryUserId;
            order.PickUpTime = command.PickUpDateTime;
            order.LastModifiedDateTime = DateTime.UtcNow;
            context.Update(order);
        }
    }
}