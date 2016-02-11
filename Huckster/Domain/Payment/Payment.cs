using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using infrastructure.Domain;

namespace Domain.Payment
{
    public class PaymentEvent: IValueObject
    {
        public int Id { get; set; }
        public Guid ParentAggregateId { get; set; }
        public string ExternalId { get; set; }
        public string Gateway { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public bool TransactionSuccess { get; set; }
        public DateTime PaymentDateTime { get; set; }
        public String ExtraInfo { get; set; }
    }
}
