using System;
using System.Collections.Generic;
using System.Data;

namespace Soul.SqlBatis.Databases
{
    public class SqlSettings
    {
        private readonly Dictionary<Type, object> _nullMappers = new Dictionary<Type, object>();
        
        private readonly Dictionary<Type, Delegate> _typeMappers = new Dictionary<Type, Delegate>();

        internal string EmptyQuerySql { get; set; }

        internal string LimitFormat { get; set; }
        
        internal string IdentifierFormat { get; set; }

        internal string IdentitySql { get; set; }
       
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
            if (_nullMappers.ContainsKey(type))
            {
                return _nullMappers[type];
            }
            return null;
        }

        public SqlSettings UseDbNullMapper<T>(T value)
        {
            _nullMappers[typeof(T)] = value;
            return this;
        }

        public SqlSettings UseTypeMapper<T>(Func<IDataRecord, int, T> mapper)
        {
            _typeMappers[typeof(T)] = mapper;
            return this;
        }
    }
}
