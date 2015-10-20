using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using DapperExtensions;
using Domain.Restaurant.Queries.Models;
using Domain.Shared;
using infrastructure.CQRS;
using infrastructure.DataAccess;

namespace Domain.Restaurant.Queries
{
    public class GetRestaurantDetailByIdQuery : IQuery<GetRestaurantDetailByIdQuery, RestaurantDetailsModel>
    {
        public int Id { get; set; }
    }

    public class GetRestaurantDetailByIdQueryHandler :
        AdoQueryHandler<GetRestaurantDetailByIdQuery, RestaurantDetailsModel>
    {
        public GetRestaurantDetailByIdQueryHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task<RestaurantDetailsModel> HandleSqlCommandAsync(IDbConnection context,
            GetRestaurantDetailByIdQuery argument)
        {
            var restaurant = context.Get<Restaurant>(argument.Id);
            var address = context.Query<Address>("Select * from [dbo].[Address] where ParentAggregateId = @ParentAggregateId", new { ParentAggregateId = restaurant.AggregateRootId}).FirstOrDefault();
            var menus = context.Query<Menu>("Select * from [dbo].[Menu] where ParentAggregateId = @ParentAggregateId", new { ParentAggregateId = restaurant.AggregateRootId }).ToList();
            var deliverySuburbs = context.Query<DeliverySuburb>("Select * from [dbo].[DeliverySuburb] where ParentAggregateId = @ParentAggregateId", new { ParentAggregateId = restaurant.AggregateRootId }).ToList();

            var deliveryHours = context.Query<DeliveryHours>("Select * from [dbo].[DeliveryHours] where ParentAggregateId = @ParentAggregateId", new { ParentAggregateId = restaurant.AggregateRootId }).ToList();

            foreach (var menu in menus)
            {
                menu.MenuItems = context.Query<MenuItem>("Select * from [dbo].[MenuItem] where MenuId = @MenuId", new { MenuId = menu.Id }).ToList();
            }

            return new RestaurantDetailsModel()
            {
                Restaurant = restaurant,
                RestauranAddress = address,
                RestaurantMenu = menus.ToList(),
                DeliverySuburbs = deliverySuburbs,
                DeliveryHours = deliveryHours
            };

        }
    }
}