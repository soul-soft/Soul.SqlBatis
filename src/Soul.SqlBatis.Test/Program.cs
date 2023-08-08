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
context.BeginTransaction();
var student = new Student()
{
    CreationTime = DateTime.Now,
    FirstName = "af"
};
context.Add(student);
await context.SaveChangesAsync();
Console.WriteLine();