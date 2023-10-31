using MySql.Data.MySqlClient;
using Soul.SqlBatis;
using Soul.SqlBatis.Entities;
using Soul.SqlBatis.Infrastructure;

var options = new DbContextOptionsBuilder()
    .EnableQueryTracking()
    .UseConnectionFactory(() => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test"))
    .Build();
var context = new MyDbContext(options);

try
{
    var param = new DynamicParameters();
    param.Add(new { Id = 23 });
    var list = context.Set<Student>(param)
        .Where(" id > @Id")
        .ToList();
    context.SaveChanges();
}
catch (Exception ex)
{

    throw;
}

