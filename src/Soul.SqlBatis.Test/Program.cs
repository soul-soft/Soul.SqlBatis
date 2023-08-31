using Microsoft.Extensions.Logging;
using MySqlConnector;
using Soul.SqlBatis;
using Soul.SqlBatis.Infrastructure;

var context = new MyDbContext(new DbContextOptions
{
    LoggerFactory = LoggerFactory.Create(logging =>
    {
        logging.AddConsole();
    }),
    DbConnection = new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test")
});
var list = new List<int>() 
{

};
var students = context.Students
    .Where(a => DbOperations.In(a.Id, list))
    .ToList();
var row = await context.SaveChangesAsync();
Console.WriteLine();