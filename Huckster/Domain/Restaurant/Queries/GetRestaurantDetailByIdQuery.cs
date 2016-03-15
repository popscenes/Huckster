using System;
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
    public class GetRestaurantDetailByAggregateRootIdQuery : IQuery<GetRestaurantDetailByAggregateRootIdQuery, RestaurantDetailsModel>
    {
        public Guid AggregateRootId { get; set; }
        public bool GetDeletedmenuItems { get; set; }
    }

    public class GetRestaurantDetailByAggregateRootIdQueryHandler :
        AdoQueryHandler<GetRestaurantDetailByAggregateRootIdQuery, RestaurantDetailsModel>
    {
        public GetRestaurantDetailByAggregateRootIdQueryHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected async override Task<RestaurantDetailsModel> HandleSqlCommandAsync(IDbConnection context, GetRestaurantDetailByAggregateRootIdQuery argument)
        {
            var restaurant =
                context.Query<Restaurant>("Select * from [dbo].[Restaurant] where [AggregateRootId] = @AggregateRootId",
                    new { AggregateRootId = argument.AggregateRootId }).FirstOrDefault();
            return RestaurantQueryHelper.GetRestaurantDetails(context, restaurant, argument.GetDeletedmenuItems);
        }
    }

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
            return RestaurantQueryHelper.GetRestaurantDetails(context, restaurant);
        }

        
    }

    public class GetRestaurantDetailWithValidHoursByIdQuery : IQuery<GetRestaurantDetailWithValidHoursByIdQuery, RestaurantDetailsModel>
    {
        public int Id { get; set; }
    }

    public class GetRestaurantDetailWithValidHoursByIdQueryHandler :
        AdoQueryHandler<GetRestaurantDetailWithValidHoursByIdQuery, RestaurantDetailsModel>
    {
        public GetRestaurantDetailWithValidHoursByIdQueryHandler(AdoContext adoContext) : base(adoContext)
        {
        }

        protected override async Task<RestaurantDetailsModel> HandleSqlCommandAsync(IDbConnection context,
            GetRestaurantDetailWithValidHoursByIdQuery argument)
        {
            var restaurant = context.Get<Restaurant>(argument.Id);

            var restaurantDetail =  RestaurantQueryHelper.GetRestaurantDetails(context, restaurant);

            var localDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById(restaurantDetail.Restaurant.TimeZoneId));
            restaurantDetail.DeliveryHours = restaurantDetail.ValidDeliveryHours(localDateTime.DayOfWeek, localDateTime.TimeOfDay);

            return restaurantDetail;
        }


    }
}