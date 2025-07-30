using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Soul.SqlBatis.Infrastructure
{
    public class GridReader : IDisposable
    {
        private IDataReader _reader;
        private IDbCommand _command;
        private bool _closeConnection;
        private bool _disposed = false;
        private EntityMappper _entityMapper;

        internal GridReader(IDbCommand command, EntityMappper entityMapper, bool closeConnection)
        {
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

        public async Task<List<T>> ReadAsync<T>()
        {
            var list = new List<T>();
            var reader = await ReadNextResultAsync();
            var mapper = _entityMapper.CreateMapper<T>(reader);
            while (await reader.ReadAsync())
            {
                var entity = mapper(reader);
                list.Add(entity);
            }
            return list;
        }

        public async Task<T> ReadFirstAsync<T>()
        {
            var reader = await ReadNextResultAsync();
            var mapper = _entityMapper.CreateMapper<T>(reader);
            while (await reader.ReadAsync())
            {
                return mapper(reader);
            }
            return default;
        }

        private IDataReader ReadNextResult()
        {
            if (_reader == null)
            {
                _reader = _command.ExecuteReader();
            }
            else
            {
                _reader.NextResult();
            }
            return _reader;
        }

        private async Task<DbDataReader> ReadNextResultAsync()
        {
            if (_reader == null)
            {
                _reader = await (_command as DbCommand).ExecuteReaderAsync();
            }
            else
            {
                var reader = _reader as DbDataReader;
                await reader.NextResultAsync();
            }
            return _reader as DbDataReader;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            try
            {
                _reader.Close();
                _reader.Dispose();
            }
            catch { }
            try
            {
                _command.Dispose();

            }
            catch { }
            try
            {
                if (_closeConnection)
                {
                    _command.Connection.Close();
                }
            }
            catch { }
            _disposed = true;
        }
    }
}
