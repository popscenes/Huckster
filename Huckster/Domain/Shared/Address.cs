using System;
using infrastructure.Domain;

namespace Domain.Shared
{
    public class Address: IValueObject, IEquatable<Address>
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

        public bool Equals(Address other)
        {
            return (this.Number.ToLower().Equals(other.Number.ToLower())) &&
                   (this.Street.ToLower().Equals(other.Street.ToLower())) &&
                   (this.Suburb.ToLower().Equals(other.Suburb.ToLower())) &&
                   (this.Postcode.ToLower().Equals(other.Postcode.ToLower())) &&
                   (this.City.ToLower().Equals(other.City.ToLower())) &&
                   (this.State.ToLower().Equals(other.State.ToLower()));
        }

        public override string ToString()
        {
            return $"{Number} {Street} {Suburb} {Postcode}";
        }
    }
}