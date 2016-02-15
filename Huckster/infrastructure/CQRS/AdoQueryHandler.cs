using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using infrastructure.DataAccess;
using NLog;

namespace infrastructure.CQRS
{
    public abstract class AdoQueryHandler<TSource, TReturn> : IQueryHandler<TSource, TReturn>
        where TSource : IQuery<TSource, TReturn>
    {
        private readonly AdoContext _adoContext;
        private static Logger logger = LogManager.GetLogger("mail");

        protected AdoQueryHandler(AdoContext adoContext)
        {
            _adoContext = adoContext;
        }

        public async Task<TReturn> HandleAsync(TSource argument)
        {

            try
            {
                using (var cn = _adoContext.GetDbConnection())
                {
                    return await HandleSqlCommandAsync(cn, argument);
                }
            }
            catch (Exception e)
            {

                logger.Log(LogLevel.Error, e.ToString() + "\n" + e.StackTrace.ToString());
                throw;
            }
            
        }


        protected abstract Task<TReturn> HandleSqlCommandAsync(IDbConnection context, TSource argument);
    }
}
