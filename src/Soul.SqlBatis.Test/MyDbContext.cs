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

		protected override void OnModelCreating(ModelBuilder builder)
		{
			//忽略字段
			builder.Entity<Student>().Igonre(a=>a.FirstName);
			//移除默认的自增规则
			builder.Entity<Student>().Property(a=>a.Id).ValueGeneratedNever();
			builder.Entity<Student>().HasKey(a => a.Id);
			builder.Entity<Student>().Property(a => a.FirstName).HasColumnName("FirstName");
		}
	}
}
