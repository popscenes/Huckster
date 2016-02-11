using System;
using System.Data;
using System.Threading.Tasks;
using DapperExtensions;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Restaurant.Commands
{
    public class UpdateRestaurantTileImage: ICommand
    {
        public int Id { get; set; }
        public string Url { get; set; }
    }

    public class UpdateRestaurantTileImageHandler : AdoCommandHandler<UpdateRestaurantTileImage>
    {
        public UpdateRestaurantTileImageHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task HandleSqlCommandAsync(IDbConnection context, UpdateRestaurantTileImage command)
        {
            Restaurant restaurant = context.Get<Restaurant>(command.Id);
            restaurant.TileImageUrl = command.Url;
            context.Update(restaurant);
        }
    }
}