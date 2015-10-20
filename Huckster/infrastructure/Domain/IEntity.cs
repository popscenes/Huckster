using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.Domain
{
    public interface IEntity
    {
        int Id { get; set; }
        Guid AggregateRootId { get; set; }
    }
}
