using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using infrastructure.Domain;

namespace Domain.Customer
{
    public class Customer: IEntity
    {
        public int Id { get; set; }
        public Guid AggregateRootId { get; set; }

        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
    }
}
