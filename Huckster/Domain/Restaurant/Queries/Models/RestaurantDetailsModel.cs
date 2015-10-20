using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Shared;

namespace Domain.Restaurant.Queries.Models
{
    public class RestaurantDetailsModel
    {
        public Restaurant Restaurant { get; set; }
        public Address RestauranAddress { get; set; }
        public List<Menu> RestaurantMenu { get; set; }

        public List<DeliverySuburb> DeliverySuburbs { get; set; }
        public List<DeliveryHours> DeliveryHours { get; set; }

    }
}
