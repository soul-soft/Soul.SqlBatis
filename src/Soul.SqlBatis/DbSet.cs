namespace Soul.SqlBatis
{
	public class DbSet<T> : DbQueryable<T>
		where T : class
	{
		public DbContext DbContext { get; }

		internal DbSet(DbContext context)
			: base(context.Model)
		{
			DbContext = context;
		}
	}
}
