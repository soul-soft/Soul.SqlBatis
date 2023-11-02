using MySql.Data.MySqlClient;
using Soul.SqlBatis;
using Soul.SqlBatis.Infrastructure;

var options = new DbContextOptionsBuilder()
    //.EnableQueryTracking()
    .UseConnectionFactory(() => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test"))
    .Build();

using var context = new MyDbContext(options);

var list1 = context.Query<decimal>("select money from student");
var list2 = context.Query<decimal>("select money from student");
Console.WriteLine(  );

