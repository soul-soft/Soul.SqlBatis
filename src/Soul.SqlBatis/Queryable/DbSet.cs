using System.Collections.Generic;

namespace Soul.SqlBatis
{
    public class DbSet<T> : DbQueryable<T> where T : class
    {
        internal DbSet(DbContext context, DynamicParameters parameters)
            : base(context, parameters)
        {

        }

        public void Add(T entity)
        {
            DbContext.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            DbContext.AddRange(entities);
        }

        public void Update(T entity)
        {
            DbContext.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            DbContext.UpdateRange(entities);
        }

        public void Remove(T entity)
        {
            DbContext.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            DbContext.RemoveRange(entities);
        }
    }
}
