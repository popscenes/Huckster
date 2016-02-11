using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using infrastructure.Domain;

namespace Domain.Restaurant
{
    public class DeliverySuburb: IValueObject
    {
        public int Id { get; set; }
        public Guid ParentAggregateId { get; set; }
        public int SuburbId { get; set; }
        public string Postcode { get; set; }
        public string Suburb { get; set; }
        public string State { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}
