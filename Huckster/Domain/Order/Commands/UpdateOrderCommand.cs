using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using Domain.Restaurant;
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

    public class UpdateOrderAntOrderItemsCommand : ICommand
    {
        public Order Order { get; set; }
        public List<OrderItem> OrderItems { get; set; }

        public class UpdateOrderAntOrderItemsCommandHandler : AdoCommandHandler<UpdateOrderAntOrderItemsCommand>
        {
            public UpdateOrderAntOrderItemsCommandHandler(AdoContext adoContext) : base(adoContext)
            {
            }

            protected async override Task HandleSqlCommandAsync(IDbConnection context,
                UpdateOrderAntOrderItemsCommand command)
            {
                Order order = context.Get<Order>(command.Order.Id);
                order.InjectFrom(command.Order);
                order.LastModifiedDateTime = DateTime.UtcNow;
                context.Update(order);

                var oldOrderItems =
                    context.Query<OrderItem>(
                        "Select * from [dbo].[OrderItem] where ParentAggregateId = @ParentAggregateId",
                        new {ParentAggregateId = command.Order.AggregateRootId}).ToList();

                foreach (var orderItem in oldOrderItems)
                {
                    context.Delete(orderItem);
                }

                var orderItems = command.OrderItems;
                orderItems.ForEach((_) =>
                {
                    _.ParentAggregateId = command.Order.AggregateRootId;
                    context.Insert(_);
                });
            }
        }
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