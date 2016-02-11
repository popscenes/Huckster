using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Shared;
using Omu.ValueInjecter;

namespace Domain.Restaurant.Queries.Models
{
    public class RestaurantDetailsModel
    {
        public Restaurant Restaurant { get; set; }
        public Address RestauranAddress { get; set; }
        public List<Menu> RestaurantMenu { get; set; }

        public List<DeliverySuburb> DeliverySuburbs { get; set; }
        public List<DeliveryHours> DeliveryHours { get; set; }

        public List<DeliveryHours> ValidDeliveryHours(DayOfWeek dayOfWeek, TimeSpan startTime)
        {
            var validhours = DeliveryHours.Where(_ => _.DayOfWeek == dayOfWeek && _.CloseTime > startTime.Add(new TimeSpan(0, 30, 0))).ToList();
            var result = new List<DeliveryHours>();

            foreach (var deliveryPeriod in validhours)
            {
                for (TimeSpan currentTime = deliveryPeriod.OpenTime;
                    currentTime <= deliveryPeriod.CloseTime;
                    currentTime = currentTime.Add(TimeSpan.FromMinutes(30)))
                {
                    var newTime = new DeliveryHours();
                    newTime.InjectFrom(deliveryPeriod);
                    newTime.OpenTime = currentTime;
                    newTime.CloseTime = currentTime.Add(TimeSpan.FromMinutes(30));
                    result.Add(newTime);
                }
            }

            return result;
        }

    }
}
