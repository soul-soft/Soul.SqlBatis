using MySql.Data.MySqlClient;
using Soul.SqlBatis;
using Soul.SqlBatis.Infrastructure;
using Soul.SqlBatis.Test;

var options = new DbContextOptionsBuilder()
    .EnableQueryTracking()
    .UseConnectionFactory(() => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test"))
    .Build();
var context = new MyDbContext(options);

try
{
    var param1 = new DynamicParameters();
    var list = context.Staffs.Where(a => DbFunctions.CountDistinct(a.Id, a.Grade) > 0).ToList();
    context.SaveChanges();
}
catch (Exception ex)
{

    throw;
}

