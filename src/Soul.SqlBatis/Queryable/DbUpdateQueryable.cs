using System;
using System.Linq.Expressions;

namespace Soul.SqlBatis
{
    public class DbUpdateQueryable<T>
    {
        public DbUpdateQueryable<T> SetProperty<TMember>(Expression<Func<T, TMember>> member, TMember value)
        {
            throw new NotImplementedException();
        }

        public DbUpdateQueryable<T> SetProperty<TMember>(Expression<Func<T, TMember>> member, Expression<Func<T, TMember>> value)
        {
            throw new NotImplementedException();
        }
    }
}
