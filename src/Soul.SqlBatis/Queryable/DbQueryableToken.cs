using System;
namespace Soul.SqlBatis
{
    public enum DbQueryableType
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

    public struct DbQueryableToken
    {
        public object Value { get; }

        public DbQueryableToken(object value)
        {
            Value = value;
        }

        public static DbQueryableToken New<TToken>(TToken token)
        {
            return new DbQueryableToken(token);
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
