using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Transactions;
using infrastructure.DataAccess;
using NLog;

namespace infrastructure.CQRS
{
    public abstract class AdoCommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : class, ICommand
    {
        private readonly AdoContext _adoContext;
        private static Logger logger = LogManager.GetLogger("mail");

        protected AdoCommandHandler(AdoContext adoContext)
        {
            _adoContext = adoContext;
        }

        public async Task HandleAsync(TCommand command)
        {
            using (TransactionScope tran = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using (var cn = _adoContext.GetDbConnection())
                {
                    try
                    {
                        cn.Open();
                        await HandleSqlCommandAsync(cn, command);
                        tran.Complete();
                        cn.Close();
                    }
                    catch (Exception e)
                    {
                        logger.Log(LogLevel.Error, e.ToString() + "\n" + e.StackTrace.ToString());
                        throw;
                    }
                    

                }
            }
        }

        protected abstract Task HandleSqlCommandAsync(IDbConnection context, TCommand command);
    }
}
