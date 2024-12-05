using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Soul.SqlBatis.Databases
{
    public class DbDataGrid : IDisposable
    {
        private IDataReader _reader;
        private IDbCommand _command;
        private bool _closeConnection;
        private readonly DbContext _context;
        private Func<IDbCommand> _createCommand;
        private readonly IEntityMapper _entityMapper;

        public DbDataGrid(DbContext context, Func<IDbCommand> createCommand, IEntityMapper entityMapper)
        {
            _context = context;
            _createCommand = createCommand;
            _entityMapper = entityMapper;
        }

        public List<T> Read<T>()
        {
            var list = new List<T>();
            var reader = ExecuteReader();
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
            var list = new List<T>();
            var reader = ExecuteReader();
            var mapper = _entityMapper.CreateMapper<T>(reader);
            while (reader.Read())
            {
                var entity = mapper(reader);
                list.Add(entity);
                break;
            }
            return list.FirstOrDefault();
        }

        public async Task<List<T>> ReadAsync<T>()
        {
            var list = new List<T>();
            var reader = await ExecuteReaderAsync();
            var mapper = _entityMapper.CreateMapper<T>(reader);
            while (reader.Read())
            {
                var entity = mapper(reader);
                list.Add(entity);
            }
            return list;
        }

        public async Task<T> ReadFirstAsync<T>()
        {
            var list = new List<T>();
            var reader = await ExecuteReaderAsync();
            var mapper = _entityMapper.CreateMapper<T>(reader);
            while (reader.Read())
            {
                var entity = mapper(reader);
                list.Add(entity);
            }
            return list.FirstOrDefault();
        }

        private IDataReader ExecuteReader()
        {
            var connection = _context.GetDbConnection();
            if (_context.GetDbConnection().State == ConnectionState.Closed)
            {
                connection.Open();
                _closeConnection = true;
            }
            if (_reader == null)
            {
                _command = _createCommand();
                _reader = _command.ExecuteReader();
            }
            else
            {
                _reader.NextResult();
            }
            return _reader;
        }

        private async Task<IDataReader> ExecuteReaderAsync()
        {
            var connection = _context.GetDbConnection();
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                _closeConnection = true;
            }
            if (_reader == null)
            {
                _command = _createCommand();
                _reader = await (_command as DbCommand).ExecuteReaderAsync();
            }
            else
            {
                _reader.NextResult();
            }
            return _reader;
        }

        ~DbDataGrid()
        {
            Dispose();
        }

        public void Dispose()
        {
            try
            {
                _reader?.Dispose();
                _reader = null;
            }
            catch { }
            try
            {
                _command?.Dispose();
                _createCommand = null;
            }
            catch { }
            try 
            {
                if (_closeConnection)
                {
                    var connection = _context.GetDbConnection();
                    connection.Close();
                    _closeConnection = false;
                }
            }
            catch { }
        }
    }
}
