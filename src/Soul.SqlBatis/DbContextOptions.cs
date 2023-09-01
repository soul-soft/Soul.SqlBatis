using System.Data;
using Microsoft.Extensions.Logging;

namespace Soul.SqlBatis
{
    public class DbContextOptions
    {
        internal DbContextOptions()
        {

        }

        public bool EnableTracking { get; internal set; }
        
        public ILoggerFactory LoggerFactory { get; internal set; }
        
        public string ConnectionString { get; internal set; }
        
        public ConnectionFactory ConnectionFactory { get; internal set; }

        internal IDbConnection CreateDbConnection()
        {
            return ConnectionFactory(ConnectionString);
        }
    }
}
