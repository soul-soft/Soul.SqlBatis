using Soul.SqlBatis.Databases;
using System;
using System.Data;

namespace Soul.SqlBatis
{
    public class DbContextOptions
    {
        internal string EmptyQuerySql { get; private set; } = "SELECT NULL";

        internal string KeywordsFormSql { get; set; }

        internal string LimitFormatSql { get; set; }

        internal string LastIdentitySql { get; private set; } 

        internal bool EnabledQueryTracking { get; private set; }

        internal IDbConnection Connection { get; private set; }

        internal Action<string, object> Loggger { get; private set; }

        internal IModel Model { get; private set; }


        public DbContextOptions UseModel(IModel model)
        {
            Model = model;
            return this;
        }

        public DbContextOptions UseNullSelectQuery(string sql)
        {
            EmptyQuerySql = sql;
            return this;
        }

        public DbContextOptions UseLogger(Action<string, object> logger)
        {
            Loggger = logger;
            return this;
        }

        public void UseNpgsql(IDbConnection connection)
        {
            Connection = connection;
            LastIdentitySql = " RETURNING {0}";
            KeywordsFormSql = "\"{0}\"";
            LimitFormatSql = "LIMIT {1} OFFSET {0}";
            EmptyQuerySql = "SELECT 1 WHERE 1 = 0";
            Model = new AnnotationModel(this);
        }

        public void UseMySql(IDbConnection connection)
        {
            Connection = connection;
            KeywordsFormSql = "`{0}`";
            LastIdentitySql = ";SELECT LAST_INSERT_ID()";
            LimitFormatSql = "LIMIT {0},{1}";
            Model = new AnnotationModel(this);
        }
    }
}
