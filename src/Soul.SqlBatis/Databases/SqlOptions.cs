using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Soul.SqlBatis.Databases
{
    public class SqlOptions
    {
        private readonly Dictionary<Type, Delegate> _typeMappers = new Dictionary<Type, Delegate>();
        private readonly Dictionary<Type, object> _nullMappers = new Dictionary<Type, object>();

        public void UseDbNullMapper<T>(Expression expression)
        {
            _nullMappers[typeof(T)] = expression;
        }

        public void UseTypeMapper<T>(Func<IDataRecord, int, T> mapper)
        {
            _typeMappers[typeof(T)] = mapper;
        }

        internal Delegate GetTypeMapper(Type type)
        {
            if (_typeMappers.ContainsKey(type))
            {
                return _typeMappers[type];
            }
            return null;
        }

        internal object GetDbNullMapper(Type type)
        {
            if (_typeMappers.ContainsKey(type))
            {
                return _nullMappers[type];
            }
            return null;
        }
    }
}
