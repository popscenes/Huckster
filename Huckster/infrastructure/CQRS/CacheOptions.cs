using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.CQRS
{
    public class CacheOptions
    {
        public string CacheKey { get; set; }
        public int CacheForMinutes { get; set; }
    }
}
