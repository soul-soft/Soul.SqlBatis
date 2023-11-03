namespace Soul.SqlBatis
{
    public class DbContextOptions
    {
        public bool EnableQueryTracking { get; set; }

        public IModelProvider ModelProvider { get; set; }

        public IDbConnectionFactory ConnectionFactory { get; set; }

        internal DbContextOptions(bool enableQueryTracking, IDbConnectionFactory connectionFactory, IModelProvider modelProvider)
        {
            ModelProvider = modelProvider;
            ConnectionFactory = connectionFactory;
            EnableQueryTracking = enableQueryTracking;
        }
    }

}
