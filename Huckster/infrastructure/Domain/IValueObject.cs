using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Domain
{
    public interface IValueObject
    {
        int Id { get; set; }
        Guid ParentAggregateId { get; set; }
    }
}
