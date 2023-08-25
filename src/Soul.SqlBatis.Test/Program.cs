using System.Text.Json;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Soul.SqlBatis;
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

var model = JsonSerializer.Deserialize<QueryModel>(json);
var list = context.Students
    .GroupBy(a => a.Name)
    .Select(s => new
    {
        s.Name,
        Count = DbFunctions.Count("*"),
    })
    .ToList();

var row = await context.SaveChangesAsync();
Console.WriteLine();