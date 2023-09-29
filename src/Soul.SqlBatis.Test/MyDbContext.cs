namespace Soul.SqlBatis.Infrastructure
{
    public partial class MyDbContext : DbContext
	{
		public MyDbContext(DbContextOptions options)
			: base(options)
		{

		}

		protected override void Logging(string sql, object param)
		{
			Console.WriteLine(sql);
		}
	}
}
