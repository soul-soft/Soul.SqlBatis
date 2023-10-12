using System;
using System.Data;

namespace Soul.SqlBatis
{
    public class DbContextOptionsBuilder
    {
        private bool _enableQueryTracking;

        private IModelProvider _modelProvider;

        private IDbConnectionFactory _connectionFactory;

        public DbContextOptionsBuilder()
        {
            _modelProvider = new DefaultModelProvider();
        }

        public DbContextOptionsBuilder EnableQueryTracking()
        {
            _enableQueryTracking = true;
            return this;
        }

        public DbContextOptionsBuilder UseConnectionFactory(Func<IDbConnection> provider)
        {
            _connectionFactory = new DelegateDbConnectionFactory(provider);
            return this;
        }

        public DbContextOptionsBuilder UserModelProvider(IModelProvider modelProvider)
        {
            _modelProvider = modelProvider;
            return this;
        }

        public DbContextOptionsBuilder UseConnectionFactory(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            return this;
        }

        public DbContextOptions Build()
        {
            return new DbContextOptions(_enableQueryTracking, _connectionFactory, _modelProvider);
        }

    }
}
