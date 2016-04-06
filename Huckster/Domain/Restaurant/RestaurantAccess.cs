using System;
using System.Collections.Generic;
using DapperExtensions.Mapper;

namespace Domain.Restaurant
{
    public class RestaurantAccess
    {
        public int Id { get; set; }
        public String UserId  { get; set; }
        public Guid RestaurantAggrgateRootId { get; set; }
        public string Name { get; set; }

    }

    public class RestaurantAccessMapper : ClassMapper<RestaurantAccess>
    {
        public RestaurantAccessMapper()
        {
            Table("RestaurantAccess");
            Map(m => m.Name).Ignore();
            AutoMap();
        }
    }
}