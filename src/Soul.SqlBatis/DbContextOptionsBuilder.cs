using System;
using System.Data;

namespace Soul.SqlBatis
{
	public class DbContextOptionsBuilder
	{
		private bool IsTracking;

		private IDbConnectionFactory ConnectionFactory;

		public DbContextOptionsBuilder UseConnectionFactory(Func<IDbConnection> provider)
		{
			ConnectionFactory = new DelegateDbConnectionFactory(provider);
			return this;
		}

		public DbContextOptionsBuilder UseConnectionFactory(IDbConnectionFactory connectionFactory)
		{
			ConnectionFactory = connectionFactory;
			return this;
		}

		public DbContextOptions Build() 
		{
			return new DbContextOptions(IsTracking, ConnectionFactory);
		}
	}
}
