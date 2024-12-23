
namespace Soul.SqlBatis.Infrastructure
{

    public partial class MyDbContext : DbContext
    {
        public MyDbContext(Action<DbContextOptions> configure) : base(configure)
        {

        }
    }
}
