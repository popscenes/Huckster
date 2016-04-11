using Dapper;
using DapperExtensions;
using Domain.Order;
using infrastructure.CQRS;
using infrastructure.DataAccess;
using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantPortal.Controllers
{
    public class RestaurantOrderAcceptCommand: ICommand
    {
        public Guid OrderAggregateRootId { get; set; }
        public TimeSpan pickUpTime { get; set; }
    }

    public class RestaurantOrderAcceptCommandHandler : AdoCommandHandler<RestaurantOrderAcceptCommand>
    {
        public RestaurantOrderAcceptCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task HandleSqlCommandAsync(IDbConnection context, RestaurantOrderAcceptCommand command)
        {
            Order order = context.Query<Order>("Select * from [dbo].[Order] where AggregateRootId = @AggregateRootId", new { AggregateRootId = command.OrderAggregateRootId }).FirstOrDefault();
            var pickUpDate = order.DeliveryTime.Date.Add(command.pickUpTime);

            order.Status = OrderStatus.RestaurantAccepted.ToString();
            order.PickUpTime = pickUpDate;
            order.LastModifiedDateTime = DateTime.UtcNow;
            context.Update(order);
        }
    }
}