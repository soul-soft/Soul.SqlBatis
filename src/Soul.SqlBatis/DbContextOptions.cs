using System;
using System.Data;

namespace Soul.SqlBatis
{
    public class DbContextOptions
    {   
        public Func<IDbConnection> ConnecionProvider { get; set; }

    }
}
