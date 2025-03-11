using Soul.SqlBatis.Infrastructure;
using System;

namespace Soul.SqlBatis
{
    public static class DbContextSettings
    {
        internal static SqlSettings MySql = new SqlSettings()
        {
            IdentifierFormat = "`{0}`",
            EmptyQuerySql = "SELECT NULL",
            LimitFormat = "LIMIT {0},{1}",
            IdentitySql = ";SELECT LAST_INSERT_ID()",
        };

        internal static SqlSettings Npgsql = new SqlSettings()
        {
            IdentifierFormat = "\"{0}\"",
            EmptyQuerySql = "SELECT 1 WHERE 1 = 0",
            LimitFormat = "LIMIT {1} OFFSET {0}",
            IdentitySql = " RETURNING {0}",
        };


        public static void Configure(DbType type, Action<SqlSettings> configure)
        {
            if (type == DbType.MySql)
            {
                configure(MySql);
            }
            else if (type == DbType.Npgsql)
            {
                configure(Npgsql);
            }
        }

        static DbContextSettings()
        {
            UseDefaultTypeMappers(MySql);
            UseDefaultTypeMappers(Npgsql);
        }

        private static void UseDefaultTypeMappers(SqlSettings settings)
        {
            settings.UseTypeMapper((record, i) =>
            {
                return record.GetInt16(i);
            });
            settings.UseTypeMapper((record, i) =>
            {
                return record.GetInt32(i);
            });
            settings.UseTypeMapper((record, i) =>
            {
                return record.GetInt64(i);
            });
            settings.UseTypeMapper((record, i) =>
            {
                return record.GetDecimal(i);
            });
            settings.UseTypeMapper((record, i) =>
            {
                return record.GetString(i);
            });
            settings.UseTypeMapper((record, i) =>
            {
                return record.GetBoolean(i);
            });
            settings.UseTypeMapper((record, i) =>
            {
                return record.GetFloat(i);
            });
            settings.UseTypeMapper((record, i) =>
            {
                return record.GetDouble(i);
            });
            settings.UseTypeMapper((record, i) =>
            {
                return record.GetDateTime(i);
            });
            settings.UseTypeMapper((record, i) =>
            {
                return record.GetGuid(i);
            });
            settings.UseTypeMapper((record, i) =>
            {
                return record.GetByte(i);
            });
            settings.UseTypeMapper((record, i) =>
            {
                return record.GetChar(i);
            });
            settings.UseTypeMapper((record, i) =>
            {
                return record.GetValue(i);
            });
        }
    }
}
