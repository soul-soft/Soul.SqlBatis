using System.Data;
using System;

namespace Soul.SqlBatis
{
	public class DbContextOptionsBuilder
	{
		private bool _isTracking;
		private IDbConnectionFactory _connectionFactory;

		public DbContextOptionsBuilder UseConnectionFactory(IDbConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
			return this;
		}

		public DbContextOptionsBuilder UseConnectionFactory(Func<IDbConnection> connectionFactory)
		{
			_connectionFactory = new DelegateDbConnectionFactory(connectionFactory);
			return this;
		}
		public DbContextOptionsBuilder AsTracking()
		{
			_isTracking = true;
			return this;
		}

		public DbContextOptions Build()
		{
			return new DbContextOptions(_isTracking, _connectionFactory);
		}

	}
}
