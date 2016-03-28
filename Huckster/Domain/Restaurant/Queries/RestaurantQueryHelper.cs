using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Domain.Restaurant.Queries.Models;
using Domain.Shared;

namespace Domain.Restaurant.Queries
{
    public class RestaurantQueryHelper
    {
        public static RestaurantDetailsModel GetRestaurantDetails(IDbConnection context, Restaurant restaurant, bool getDeletedMenuItems = false)
        {
            var address =
                context.Query<Address>("Select * from [dbo].[Address] where ParentAggregateId = @ParentAggregateId",
                    new { ParentAggregateId = restaurant.AggregateRootId }).FirstOrDefault();
            
            var deliverySuburbs =
                context.Query<DeliverySuburb>(
                    "Select * from [dbo].[DeliverySuburb] where ParentAggregateId = @ParentAggregateId",
                    new { ParentAggregateId = restaurant.AggregateRootId }).ToList();

            var deliveryHours =
                context.Query<DeliveryHours>(
                    "Select * from [dbo].[DeliveryHours] where ParentAggregateId = @ParentAggregateId",
                    new { ParentAggregateId = restaurant.AggregateRootId }).ToList();

            var menus =
                context.Query<Menu>("Select * from [dbo].[Menu] where ParentAggregateId = @ParentAggregateId",
                    new { ParentAggregateId = restaurant.AggregateRootId }).ToList();

            if (!getDeletedMenuItems)
            {
                menus = menus.Where(_ => _.Deleted == false).ToList();
            }

            foreach (var menu in menus)
            {
                menu.MenuItems =
                    context.Query<MenuItem>("Select * from [dbo].[MenuItem] where MenuId = @MenuId", new { MenuId = menu.Id })
                        .ToList();

                if (!getDeletedMenuItems)
                {
                    menu.MenuItems = menu.MenuItems.Where(_ => _.Deleted == false).ToList();
                }
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
