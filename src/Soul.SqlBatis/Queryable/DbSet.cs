using System.Collections.Generic;

namespace Soul.SqlBatis
{
    public class DbSet<T> : DbQueryable<T> where T : class
    {
        protected DbContext _context;

        internal DbSet(DbContext context, DynamicParameters parameters)
            : base(context, typeof(T), parameters)
        {

        }

        public void Add(T entity)
        {
            _context.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _context.AddRange(entities);
        }

        public void Update(T entity)
        {
            _context.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _context.UpdateRange(entities);
        }

        public void Remove(T entity)
        {
            _context.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.RemoveRange(entities);
        }
    }
}
