using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Order
{
    public class PrintQueue
    {
        public int Id { get; set; }
		public int RestaurantId { get; set; }
        public Guid RestaurantAggrgateRootId { get; set; }
        public Guid OrderAggrgateRootId { get; set; }
        public DateTime DateTimeAdded { get; set; }
        public bool Printed { get; set; }
        public string PrintRequestXML { get; set; }
    }
}
