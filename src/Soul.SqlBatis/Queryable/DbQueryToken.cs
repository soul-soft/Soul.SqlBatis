using System;
namespace Soul.SqlBatis
{
    public enum DbQueryMethod
    {
        As,
        Take,
        Skip,
        Where,
        OrderBy,
        OrderByDescending,
        GroupBy,
        Having,
        Select,
        Setters
    }

    public struct DbQueryToken
    {
        public object Value { get; }

        public DbQueryToken(object value)
        {
            Value = value;
        }

        public static DbQueryToken New<TToken>(TToken token)
        {
            return new DbQueryToken(token);
        }
        
        public T As<T>()
        {
            if (Value is T expression)
            {
                return expression;
            }
            throw new NotImplementedException();
        }

        public bool Is<T>()
        {
            if (Value is T)
            {
                return true;
            }
            return false;
        }
    }
}
