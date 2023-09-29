using System;
using System.Data;

namespace Soul.SqlBatis
{
	public class DbContextOptions
	{
		public bool IsTracking { get; }

		public IDbConnectionFactory ConnectionFactory { get; }

		public DbContextOptions(bool isTracking, IDbConnectionFactory connectionFactory)
		{
			IsTracking = isTracking;
			ConnectionFactory = connectionFactory;
		}
	}
}
