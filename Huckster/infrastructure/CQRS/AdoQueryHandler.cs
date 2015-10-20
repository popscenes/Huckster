using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using infrastructure.DataAccess;

namespace infrastructure.CQRS
{
    public abstract class AdoQueryHandler<TSource, TReturn> : IQueryHandler<TSource, TReturn>
        where TSource : IQuery<TSource, TReturn>
    {
        private readonly AdoContext _adoContext;

        protected AdoQueryHandler(AdoContext adoContext)
        {
            _adoContext = adoContext;
        }

        public async Task<TReturn> HandleAsync(TSource argument)
        {
            
            using (var cn = _adoContext.GetDbConnection())
            {
                return await HandleSqlCommandAsync(cn, argument);
            }
        }


        protected abstract Task<TReturn> HandleSqlCommandAsync(IDbConnection context, TSource argument);
    }
}
