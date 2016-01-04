using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
using infrastructure.Domain;
using Omu.ValueInjecter;

namespace Domain.Restaurant
{
    public class Restaurant: IEntity
    {
        public Restaurant()
        {
        }

        public int Id { get; set; }
        public Guid AggregateRootId { get; set; }
        public string Name { get;  set; }
        public string Description { get; set; }
        

        public string TimeZoneId { get; set; }
        public string TileImageUrl { get; set; }

        public string FullTileImageUrl {
            get
            {
                return ConfigurationManager.AppSettings["RestaurantBaseUrl"] + TileImageUrl;
            }
        }
    }

    public class RestaurantMapper : ClassMapper<Restaurant>
    {
        public RestaurantMapper()
        {
            Table("Restaurant");
            Map(m => m.FullTileImageUrl).Ignore();
            AutoMap();
        }
    }
    public enum ServiceType
    {
        Breakfast,
        Lunch,
        Dinner
    }


}
