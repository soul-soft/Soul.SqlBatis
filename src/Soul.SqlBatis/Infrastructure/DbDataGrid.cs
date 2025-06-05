using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Soul.SqlBatis.Infrastructure
{
    public class DbDataGrid : IDisposable
    {
        private IDataReader _reader;
        private bool _closeConnection;
        private IDbCommand _command;
        private EntityMappper _entityMapper;

        internal DbDataGrid(IDbCommand command, IDataReader reader, EntityMappper entityMapper, bool closeConnection)
        {
            _reader = reader;
            _command = command;
            _entityMapper = entityMapper;
            _closeConnection = closeConnection;
        }

        public List<T> Read<T>()
        {
            var list = new List<T>();
            var reader = ReadNextResult();
            var mapper = _entityMapper.CreateMapper<T>(reader);
            while (reader.Read())
            {
                var entity = mapper(reader);
                list.Add(entity);
            }
            return list;
        }

        public T ReadFirst<T>()
        {
            var reader = ReadNextResult();
            var mapper = _entityMapper.CreateMapper<T>(reader);
            while (reader.Read())
            {
                return mapper(reader);
            }
            return default;
        }

        public  Task<List<T>> ReadAsync<T>()
        {
            var list = new List<T>();
            var reader = ReadNextResult();
            var mapper = _entityMapper.CreateMapper<T>(reader);
            while (reader.Read())
            {
                var entity = mapper(reader);
                list.Add(entity);
            }
            return Task.FromResult(list);
        }

        public Task<T> ReadFirstAsync<T>()
        {
            var reader = ReadNextResult();
            var mapper = _entityMapper.CreateMapper<T>(reader);
            while (reader.Read())
            {
                return Task.FromResult(mapper(reader));
            }
            return Task.FromResult<T>(default);
        }

        private IDataReader ReadNextResult()
        {
            _reader.NextResult();
            return _reader;
        }

        public void Dispose()
        {
            try
            {
                _reader?.Dispose();
                _reader?.Close();
                _reader = null;
            }
            catch { }
            try
            {
                _command?.Dispose();
                _command = null;
            }
            catch { }
            try 
            {
                if (_closeConnection)
                {
                    var connection = _command.Connection;
                    connection.Close();
                    _closeConnection = false;
                }
            }
            catch { }
        }
    }
}
