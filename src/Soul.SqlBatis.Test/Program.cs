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

var student = new Student()
{
    FirstName = "Test",
    Name = "Test",
    CreationTime = DateTime.Now,
};
context.Add(student);
var list = context.Students.Take(10).Skip(10).ToList();
await context.SaveChangesAsync();
Console.WriteLine();