using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using infrastructure.DataAccess;

namespace infrastructure.CQRS
{
    public abstract class AdoCommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : class, ICommand
    {
        private readonly AdoContext _adoContext;

        protected AdoCommandHandler(AdoContext adoContext)
        {
            _adoContext = adoContext;
        }

        public async Task HandleAsync(TCommand command)
        {
            using (var cn = _adoContext.GetDbConnection())
            {
                cn.Open();
                await HandleSqlCommandAsync(cn, command);
                cn.Close();
            }
        }

        protected abstract Task HandleSqlCommandAsync(IDbConnection context, TCommand command);
    }
}
