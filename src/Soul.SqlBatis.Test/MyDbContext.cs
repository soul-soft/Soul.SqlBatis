namespace Soul.SqlBatis.Infrastructure
{
    public partial class MyDbContext : DbContext
	{
		public MyDbContext(DbContextOptions options)
			: base(options)
		{

		}
	}
}
