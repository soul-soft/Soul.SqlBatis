using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Soul.SqlBatis
{
    public abstract class DbContext : IDisposable
	{
		private IDbConnection _connection;
		
		private DbContextTransaction _transaction;

		public DbContextTransaction CurrentDbTransaction => _transaction;

		public DbSet<T> Set<T>()
			where T : class
		{
			return new DbSet<T>(this);
		}

		public IDbConnection GetDbConnection()
		{
			return _connection;
		}

		public IDbTransaction GetDbTransaction()
		{
			return CurrentDbTransaction?.GetDbTransaction();
		}

		public void OpenDbConnection()
		{
			if (_connection.State == ConnectionState.Closed)
			{
				_connection.Open();
			}
		}

		public async Task OpenDbConnectionAsync()
		{
			if (_connection.State == ConnectionState.Closed)
			{
				if (_connection is DbConnection connection)
				{
					await connection.OpenAsync();
				}
				else
				{
					_connection.Open();
				}
			}
		}

		public void ColseDbConnection()
		{
			if (_connection.State != ConnectionState.Closed)
			{
				_connection.Close();
			}
		}

		public Task ColseDbConnectionAsync()
		{
			ColseDbConnection();
			return Task.CompletedTask;
		}

		public void Dispose()
		{
			_connection?.Close();
			_connection?.Dispose();
            _connection = null;
		}
	}
}
