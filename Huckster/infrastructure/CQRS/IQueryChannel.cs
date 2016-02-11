using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.CQRS
{
    public interface IQueryChannel
    {
        Task<TReturn> QueryAsync<TQuery, TReturn>(IQuery<TQuery, TReturn> argument, TReturn defaultReturn = default(TReturn))
             where TQuery : IQuery<TQuery, TReturn>;
    }
}
