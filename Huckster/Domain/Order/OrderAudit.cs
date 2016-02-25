using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using infrastructure.Domain;

namespace Domain.Order
{
    public class OrderAudit: IValueObject
    {
        public int Id { get; set; }
        public Guid ParentAggregateId { get; set; }                    
        public string Action { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string UserName { get; set; }
    }
}
