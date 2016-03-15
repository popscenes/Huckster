using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using infrastructure.Domain;

namespace Domain.Restaurant
{
    public class DeliveryHours : IValueObject
    {
        public int Id { get; set; }
        public Guid ParentAggregateId { get; set; }

        public ServiceType ServiceType { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }

        public String TimeZoneId { get; set; }

        public string ToIsoString()
        {
            int dayTofind= (int) DayOfWeek;
            int current = (int) DateTime.Today.DayOfWeek;
            int daysToAdd = 0;
            if (current > dayTofind)
            {
                daysToAdd = 7 - current + dayTofind;
            }
            else
            {
                daysToAdd = dayTofind - current;
            }
            return
                DateTime.SpecifyKind(DateTime.Today.AddDays(daysToAdd).Add(OpenTime), DateTimeKind.Unspecified)
                    .ToString("yyyy-MM-ddTHH:mm:ss");
        }

        public string ToTimeString()
        {
            DateTime time = DateTime.Today.Add(OpenTime);
            return time.ToString("hh:mm tt");
        }
    }
}
