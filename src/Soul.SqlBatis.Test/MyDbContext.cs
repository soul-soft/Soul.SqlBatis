using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis.Test
{
	public class MyDbContext : DbContext
	{
		public MyDbContext(DbContextOptions options)
			: base(options)
		{

		}

		public DbSet<Student> Students => Set<Student>();
	}
}
