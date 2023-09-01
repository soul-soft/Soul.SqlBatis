using System.Data;

namespace Soul.SqlBatis
{
    public delegate IDbConnection ConnectionFactory(string connectionString);
   
    public enum ConnectionProvider
    {
        MySql,
        SqlServer,
        Sqllite
    }
}
