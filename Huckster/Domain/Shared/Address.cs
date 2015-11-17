using System;
using infrastructure.Domain;

namespace Domain.Shared
{
    public class Address: IValueObject
    {
        public Address()
        {

        }

        public int Id { get; set; }
        public Guid ParentAggregateId { get; set; }

        public String Number { get; set; }
        public String Street { get; set; }
        public String Suburb { get; set; }
        public String Postcode { get; set; }
        public String City { get; set; }
        public String State { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString()
        {
            return $"{Number} {Street} {Suburb} {Postcode}";
        }
    }
}