using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace infrastructure.DataAccess
{
    public class AdoContext
    {
        public string DatabaseName { get; set; }

        public IDbConnection GetDbConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings[DatabaseName].ConnectionString;
            SqlConnection cn = new SqlConnection(connectionString);
            return cn;
        }
    }
}
