using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DapperExtensions.Mapper;
using infrastructure.Domain;

namespace Domain.Restaurant
{
    public class Restaurant: IEntity
    {
        public Restaurant()
        {
        }

        public List<DeliveryHours> GetValidDeliveryHours(List<DeliveryHours> deliveryHours, DayOfWeek dayOfWeek, TimeSpan startTime)
        {
            return deliveryHours.Where(_ => _.DayOfWeek == dayOfWeek && _.OpenTime > startTime.Add(new TimeSpan(0,30,0))).ToList();
        }

        public int Id { get; set; }
        public Guid AggregateRootId { get; set; }
        public string Name { get;  set; }
        public string Description { get; set; }
        

        public string TimeZoneId { get; set; }
        public string TileImageUrl { get; set; }
    }
    public enum ServiceType
    {
        Breakfast,
        Lunch,
        Dinner
    }


}
