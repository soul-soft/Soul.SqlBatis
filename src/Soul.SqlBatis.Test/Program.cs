using System.Text.Json;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Soul.SqlBatis;
using Soul.SqlBatis.Entities;
using Soul.SqlBatis.Infrastructure;
using Soul.SqlBatis.Test;

var context = new MyDbContext(new DbContextOptions
{
    LoggerFactory = LoggerFactory.Create(logging =>
    {
        logging.AddConsole();
    }),
    DbConnection = new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test")
});

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