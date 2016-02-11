using System.Data;
using System.Threading.Tasks;
using DapperExtensions;
using Domain.Shared;
using infrastructure.CQRS;
using infrastructure.DataAccess;
using Omu.ValueInjecter;

namespace Domain.Restaurant.Commands
{
    public class UpdateRestaurantDetailsCommand: ICommand
    {
        public Restaurant Restaurant { get; set; }
        public int Id { get; set; }
    }

    public class UpdateRestaurantDetailsCommandHandler : AdoCommandHandler<UpdateRestaurantDetailsCommand>
    {
        public UpdateRestaurantDetailsCommandHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task HandleSqlCommandAsync(IDbConnection context, UpdateRestaurantDetailsCommand command)
        {
            var restaurant = context.Get<Restaurant>(command.Id);
            restaurant.InjectFrom(command.Restaurant);
            context.Update(restaurant);
        }
    }
}