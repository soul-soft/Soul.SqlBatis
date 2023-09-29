using System.Text.Json;
using MySqlConnector;
using Soul.SqlBatis;
using Soul.SqlBatis.Entities;
using Soul.SqlBatis.Infrastructure;
using Soul.SqlBatis.Test;

var options = new DbContextOptionsBuilder()
    .AsTracking()
	.UseConnectionFactory(() => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test"))
    .Build();

var context = new MyDbContext(options);

var json = "{\"Orders\":{\"Id\":1,\"Name\":1}}";

var model = JsonSerializer.Deserialize<QueryModel>(json);
var student = context.Students
    .AsTracking()
    .First();
student.Address = new Address
{
    P = "浙江",
    C = "宁波"
};
var row = await context.SaveChangesAsync();
Console.WriteLine();