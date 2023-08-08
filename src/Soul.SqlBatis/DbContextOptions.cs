using System;
using System.Data;
using Microsoft.Extensions.Logging;

namespace Soul.SqlBatis
{
    public class DbContextOptions
    {
        public ILoggerFactory LoggerFactory { get; set; }
        public Func<IDbConnection> ConnecionProvider { get; set; }
    }
}
