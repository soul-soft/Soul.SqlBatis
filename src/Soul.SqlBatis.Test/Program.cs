using System.Text.Json;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Soul.SqlBatis;
using Soul.SqlBatis.Infrastructure;
using Soul.SqlBatis.Test;

var context = new MyDbContext(new DbContextOptions
{
    LoggerFactory = LoggerFactory.Create(logging =>
    {
        logging.AddConsole();
    }),
    ConnecionProvider = () => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test")
});

var json = "{\"Orders\":{\"Id\":1,\"Name\":1}}";

var context = new MyDbContext(options);
var context1 = new MyDbContext(options);
var list = new List<int>()
{

};
var students = context.Students
    .Where(a => a.CreationTime > DateTime.Now)
    .ToList();
var row = await context.SaveChangesAsync();
Console.WriteLine();