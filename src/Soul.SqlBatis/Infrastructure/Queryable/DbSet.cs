namespace Soul.SqlBatis
{
    public class DbSet<T> : DbQueryable<T>
        where T : class
    {
        internal DbSet(DbContext context)
            : base(context, typeof(T))
        {

        }
    }
}
