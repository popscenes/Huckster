using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using infrastructure.Domain;

namespace Domain.Restaurant
{
    public class OpenHours: IValueObject
    {
        public int Id { get; set; }
        public Guid ParentAggregateId { get; set; }

        public ServiceType ServiceType { get; set; }

        public DayOfWeek DayOfWeek { get; set; }
        public DateTime OpenTime { get; set; }
        public DateTime CloseTime { get; set; }

        public String TimeZoneId { get; set; }
    }
}