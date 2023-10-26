using MySql.Data.MySqlClient;
using Soul.SqlBatis;
using Soul.SqlBatis.Infrastructure;

var options = new DbContextOptionsBuilder()
    .EnableQueryTracking()
    .UseConnectionFactory(() => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test"))
    .Build();
var context = new MyDbContext(options);

try
{
    var student = context.Students.Single();
    context.SaveChanges();
}
catch (Exception ex)
{

    throw;
}

