namespace Soul.SqlBatis.Infrastructure
{
    public partial class MyDbContext : DbContext
	{
		public MyDbContext(Action<DbContextOptionsBuilder> configure)
			: base(configure)
		{

		}
	}
}
