using MySql.Data.MySqlClient;
using Soul.SqlBatis.Entities;

namespace Soul.SqlBatis.Infrastructure
{
    public partial class MyDbContext : DbContext
    {
        public MyDbContext()
        {

        }

        public MyDbContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void Logging(string sql, object param)
        {
            Console.WriteLine(sql);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Student>().Property(a => a.Rowversion).IsConcurrencyToken();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseConnectionFactory(() => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test"));
        }
    }
}
