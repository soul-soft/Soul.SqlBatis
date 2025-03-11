using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soul.SqlBatis.Test
{
    public static class Fata
    {
        public static void F()
        {
            DbContextSettings.Configure(DbType.Npgsql, configure =>
            {
                configure.UseTypeMapper((record, i) =>
                {
                    return ((NpgsqlDataReader)record).GetFieldValue<int[]>(i);
                });
            });
        }
    }
}
