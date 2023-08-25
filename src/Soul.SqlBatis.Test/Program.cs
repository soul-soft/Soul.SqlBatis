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
var (list, count) = context.Students
    .Where(a => a.Id > 10)
    .OrderBy(model)
    .ToPageList(1, 10);

var row = await context.SaveChangesAsync();
Console.WriteLine();