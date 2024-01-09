namespace Soul.SqlBatis
{
    public class DbSet<T> : DbQueryable<T>
        where T : class
    {
        internal DbSet(DbContext context, DynamicParameters parameters)
            : base(context, typeof(T), parameters)
        {

        }

        internal DbSet(DbContext context, string fromSql, DynamicParameters parameters)
            : base(context, typeof(T), fromSql, parameters)
        {

        }

        internal DbSet(DbContext context, SqlBuilder whereBuilder, DynamicParameters parameters)
           : base(context, typeof(T), whereBuilder, parameters)
        {

        }
    }
}
