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

var student = context.Students
    .Where(a => a.Id == 6)
    .AsTracking()
    .FirstOrDefault();
student.Name = "zs1ddd11";
var ent =  context.Entry(student).Entity;
context.SaveChanges();

Console.WriteLine();