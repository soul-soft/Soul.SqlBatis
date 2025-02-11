using Soul.SqlBatis.ChangeTracking;
using Soul.SqlBatis.Databases;
using System;
using System.Data;

namespace Soul.SqlBatis
{
    public class DbContextOptions
    {
        internal string EmptyQuerySql { get; private set; } = "SELECT NULL";

        internal string LastIdentitySql { get; private set; } = "SELECT LAST_INSERT_ID()";

        internal bool QueryTracking { get; private set; }

        internal IDbConnection Connection { get; private set; }

        internal Action<string, object> Loggger { get; private set; }

        internal IModel Model { get; private set; }

        internal IDbContextPersister Persister { get; private set; }

        internal IEntityMapper EntityMapper { get; private set; } = new EntityMapper(new EntityMapperOptions());

        internal SqlBuilderOptions SqlBuilderOptions { get; private set; } = new SqlBuilderOptions();

        public DbContextOptions UseModel(IModel model)
        {
            Model = model;
            return this;
        }

        public DbContextOptions UseEntityMapper(IEntityMapper entityMapper)
        {
            EntityMapper = entityMapper;
            return this;
        }

        public DbContextOptions UseEntityMapper(Action<EntityMapperOptions> configureOptions)
        {
            var options = new EntityMapperOptions();
            configureOptions(options);
            EntityMapper = new EntityMapper(options);
            return this;
        }

        public DbContextOptions UseSqlBuilder(Action<SqlBuilderOptions> configureOptions)
        {
            configureOptions?.Invoke(SqlBuilderOptions);
            return this;
        }

        public DbContextOptions UseQueryTracking()
        {
            QueryTracking = true;
            return this;
        }

        public DbContextOptions UseEntityPersister(IDbContextPersister persister)
        {
            Persister = persister;
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

        public void UsePgSql(IDbConnection connection)
        {
            Connection = connection;
            Persister = new DbContextPersister(this);
            LastIdentitySql = " RETURNING {0};";
            SqlBuilderOptions.LimitFormat = "LIMIT {0} OFFSET {1}";
            Model = new AnnotationModel("\"{0}\"");
        }

        public void UseMySql(IDbConnection connection)
        {
            Connection = connection;
            Persister = new DbContextPersister(this);
            LastIdentitySql = ";SELECT LAST_INSERT_ID()";
            SqlBuilderOptions.LimitFormat = "LIMIT {0},{1}";
            Model = new AnnotationModel("\"{0}\"");
        }
    }
}
