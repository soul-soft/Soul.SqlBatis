using System;
using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis.Exceptions
{
    public class DbUpdateConcurrencyException : Exception
    {
        public EntityEntry Entity { get; }

        public DbUpdateConcurrencyException(string message, EntityEntry entity) 
            : base(message)
        {
            Entity = entity;
        }
    }
}
