using Dapper;
using DapperExtensions;
using Domain.Order;
using Domain.Restaurant;
using infrastructure.CQRS;
using infrastructure.DataAccess;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Domain.Order.Commands
{
    public class AddToPrintQueueCommand : ICommand
    {
        public Order Order { get; set; }
        public string OrderPrinetRequest { get; set; }
    }

    public class AddToPrintQueueCommandHandler : AdoCommandHandler<AddToPrintQueueCommand>
    {
        public AddToPrintQueueCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task HandleSqlCommandAsync(IDbConnection context, AddToPrintQueueCommand command)
        {

            var restaurant =
            context.Query<Restaurant.Restaurant>("Select * from [dbo].[Restaurant] where [AggregateRootId] = @AggregateRootId",
                new { AggregateRootId = command.Order.RestaurantId }).FirstOrDefault();

            var queueItem = new PrintQueue()
            {
                DateTimeAdded  = DateTime.Now,
                OrderAggrgateRootId = command.Order.AggregateRootId,
                Printed = false,
                RestaurantAggrgateRootId = command.Order.RestaurantId,
                PrintRequestXML = command.OrderPrinetRequest,
                RestaurantId = restaurant.Id

            };
            context.Insert(queueItem);
        }
    }
    
}