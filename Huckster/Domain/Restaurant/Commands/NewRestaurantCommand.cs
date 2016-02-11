using System;
using System.Data;
using System.Threading.Tasks;
using DapperExtensions;
using infrastructure.CQRS;
using infrastructure.DataAccess;


namespace Domain.Restaurant.Commands
{
    public class NewRestaurantCommand : ICommand
    {
        public Restaurant Restaurant { get; set; }
    }

    public class NewRestaurantCommandHandler : AdoCommandHandler<NewRestaurantCommand>
    {
        public NewRestaurantCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task HandleSqlCommandAsync(IDbConnection context, NewRestaurantCommand command)
        {
            command.Restaurant.TimeZoneId = "AUS Eastern Standard Time";
            command.Restaurant.AggregateRootId = Guid.NewGuid();
            command.Restaurant.TileImageUrl = "";
            context.Insert(command.Restaurant);
        }
    }
}