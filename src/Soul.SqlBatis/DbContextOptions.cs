using Soul.SqlBatis.ChangeTracking;
using System;
using System.Data;

namespace Soul.SqlBatis
{
    public class DbContextOptions
    {
        internal string EmptyQuerySql { get; private set; } = "SELECT NULL";

        internal bool QueryTracking { get; private set; }

        internal IDbConnection Connection { get; private set; }

        internal Action<string, object> Loggger { get; private set; }

        internal IModel Model { get; private set; } = new AnnotationModel("`{0}`");

        internal IEntityPersister EntityPersister { get; private set; } = new EntityPersister();

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

        public DbContextOptions UseConnection(IDbConnection connection)
        {
            Connection = connection;
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

        public DbContextOptions UseEntityPersister(IEntityPersister entityPersister)
        {
            EntityPersister = entityPersister;
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
    }
}
