using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Soul.SqlBatis
{
	public static class IDbQueryableExtensions
	{
		public static List<T> ToList<T>(this IDbQueryable<T> queryable)
			where T : class
		{
			if (!(queryable is DbQueryable))
			{
				throw new InvalidOperationException();
			}
			var query = queryable as DbQueryable;
			return query.ToList<T>();
		}

		public static Task<List<T>> ToListAsync<T>(this IDbQueryable<T> queryable)
		{
			if (!(queryable is DbQueryable))
			{
				throw new InvalidOperationException();
			}
			var query = queryable as DbQueryable;
			return query.ToListAsync<T>();
		}

        public static List<T> Count<T>(this IDbQueryable<T> queryable)
             where T : class
        {
            if (!(queryable is DbQueryable))
            {
                throw new InvalidOperationException();
            }
            var query = queryable as DbQueryable;
            return query.ToList<T>();
        }
    }
}
