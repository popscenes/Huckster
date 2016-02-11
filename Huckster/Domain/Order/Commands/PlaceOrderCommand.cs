using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DapperExtensions;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Order
{
    public class PlaceOrderCommand: ICommand
    {
        public Order Order { get; set; }
        public List<OrderItem> OrderItems { get; set; }
    }

    public class PlaceOrderCommandHandler : AdoCommandHandler<PlaceOrderCommand>
    {
        public PlaceOrderCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task HandleSqlCommandAsync(IDbConnection context, PlaceOrderCommand command)
        {
            command.Order.CreateDateTime = DateTime.UtcNow;
            command.Order.LastModifiedDateTime = DateTime.UtcNow;
            command.Order.Status = OrderStatus.Placed.ToString();

            var orderId = context.Insert(command.Order);

            var orderItems = command.OrderItems;
            orderItems.ForEach((_) => { _.ParentAggregateId = command.Order.AggregateRootId; context.Insert(_); });
            
        }
    }
}