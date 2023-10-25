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

    var param1 = new DynamicParameters();
    var list = context.Students
        .Select(s => new
         {
             Status = DbOperations.Switch(a => a.Case(s.State == 0, "初始").Case(s.State == 1, "正式"), "游客")
         }).ToList();
  
    context.SaveChanges();
}
catch (Exception ex)
{

    throw;
}

