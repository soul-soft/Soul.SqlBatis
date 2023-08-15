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
    .Where(a => DbFunctions.JsonExtract<string>(a.Address, "$.P") == "江西1")
    .First();
student.FirstName = "王";
student.Address = new Address()
{
    P = "江西2",
    C = "上饶2"
};
var stu = context.Entry(student);
var row = await context.SaveChangesAsync();
Console.WriteLine();