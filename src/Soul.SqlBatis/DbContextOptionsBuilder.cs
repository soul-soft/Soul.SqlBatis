using System.Data;
using Microsoft.Extensions.Logging;

namespace Soul.SqlBatis
{
    public class DbContextOptionsBuilder
    {
        private readonly DbContextOptions _options;

        internal DbContextOptionsBuilder()
        {
            _options = new DbContextOptions();
        }

        public DbContextOptionsBuilder EnableTracking()
        {
            _options.Tracking = true;
            return this;
        }

        public DbContextOptionsBuilder EnableNoTracking()
        {
            _options.Tracking = false;
            return this;
        }

        public DbContextOptionsBuilder UseMySql(IDbConnection connection)
        {
            _options.DbConnection = connection;
            return this;
        }

        public DbContextOptionsBuilder UseLoggerFactory(ILoggerFactory loggerFactory)
        {
            _options.LoggerFactory = loggerFactory;
            return this;
        }

        internal DbContextOptions Build()
        {
            return _options;    
        }
    }
}
