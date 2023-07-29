using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis.Test
{
	public class MyDbContext : DbContext
	{
        public MyDbContext() 
        {

        }

        public DbSet<Student> Students => Set<Student>();

        protected override void OnModelCreating(ModelBuilder builder)
        {

        }
    }
}
