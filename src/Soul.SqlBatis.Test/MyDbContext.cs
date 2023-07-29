
namespace Soul.SqlBatis.Test
{
	public class MyDbContext : DbContext
	{
        public DbSet<Student> Students => Set<Student>();
    }
}
