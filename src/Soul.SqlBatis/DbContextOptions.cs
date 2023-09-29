namespace Soul.SqlBatis
{
	public class DbContextOptions
    {
        public bool IsTracking { get; set; }
      
		public IDbConnectionFactory ConnectionFactory { get; set; }

		public DbContextOptions(bool isTracking, IDbConnectionFactory connectionFactory)
		{
			IsTracking = isTracking;
			ConnectionFactory = connectionFactory;
		}
	}
}
