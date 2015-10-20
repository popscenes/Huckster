using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.CQRS
{
    public interface IQueryHandler<in TSource, TReturn>
        where TSource : IQuery<TSource, TReturn>
    {
        Task<TReturn> HandleAsync(TSource argument);
    }
}
