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
    .AsTracking()
    .Where(a => a.Id == 6)
    .First();
student.FirstName = "王2";
//student.Address.Add(new Address 
//{
//    P = "faf"
//});
var stu = context.Entry(student);
var row = await context.SaveChangesAsync();
Console.WriteLine();