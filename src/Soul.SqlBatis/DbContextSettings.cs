using Soul.SqlBatis.Databases;
using System;

namespace Soul.SqlBatis
{
    public class DbContextSettings
    {
        public SqlSettings MySql = new SqlSettings()
        {
            IdentifierFormat = "`{0}`",
            EmptyQuerySql = "SELECT NULL",
            LimitFormat = "LIMIT {0},{1}",
            IdentitySql = ";SELECT LAST_INSERT_ID()",
        };

        public SqlSettings Npgsql = new SqlSettings()
        {
            IdentifierFormat = "\"{0}\"",
            EmptyQuerySql = "SELECT 1 WHERE 1 = 0",
            LimitFormat = "LIMIT {1} OFFSET {0}",
            IdentitySql = " RETURNING {0}",
        };

        public void ConfigureMysql(DbType dbType,Action<SqlSettings> settings)
        {
            if (dbType == DbType.MySql)
            {
                settings(MySql);
            }
            else if (dbType == DbType.Npgsql)
            {
                settings(Npgsql);
            }
        }
    }
}
