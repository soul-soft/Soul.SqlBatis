
namespace Soul.SqlBatis.Test
{

    internal class MyDbContext : DbContext
    {
        public MyDbContext(Action<DbContextOptions> configure) : base(configure)
        {

        }
    }
}
