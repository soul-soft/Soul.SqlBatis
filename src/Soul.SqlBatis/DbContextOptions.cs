using System;
using System.Data;

namespace Soul.SqlBatis
{
    public class DbContextOptions
    {
        internal DbType DbType { get; private set; }

        internal IDbConnection Connection { get; private set; }

        internal Action<string, object> Loggger { get; private set; }

        public DbContextOptions UseLogger(Action<string, object> logger)
        {
            Loggger = logger;
            return this;
        }
      
        public void UseNpgsql(IDbConnection connection)
        {
            Connection = connection;
            DbType = DbType.Npgsql;
        }

        public void UseMySql(IDbConnection connection)
        {
            Connection = connection;
            DbType = DbType.MySql;
        }
    }


}
