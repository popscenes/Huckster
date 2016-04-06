using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Restaurant.Commands
{
    public class SetRestaurantAccessCommand: ICommand
    {
        public string UserId { get; set; }
        public Guid restaurantAggregateRoodId { get; set; }
    }

    public class SetRestaurantAccessCommandHandler : AdoCommandHandler<SetRestaurantAccessCommand>
    {
        public SetRestaurantAccessCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task HandleSqlCommandAsync(IDbConnection context, SetRestaurantAccessCommand command)
        {
            context.Query("Delete from [dbo].[RestaurantAccess] where UserId = @UserId", new {UserId = command.UserId});
            context.Insert(new RestaurantAccess()
            {
                UserId = command.UserId,
                RestaurantAggrgateRootId = command.restaurantAggregateRoodId
            });
            
        }
    }
}