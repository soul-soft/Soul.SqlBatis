using System;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis.Exceptions
{
    public class DbUpdateConcurrencyException : Exception
    {
        public IEntityEntry Entity { get; }

        public DbUpdateConcurrencyException(string message, IEntityEntry entity) 
            : base(message)
        {
            Entity = entity;
        }
    }
}
