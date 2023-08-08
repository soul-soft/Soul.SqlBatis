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


var sql = @"
SELECT 
    FirstName,
    Count(*) Count
FROM
    students
GROUP BY
    FirstName
";
var list = context.FromSql<StudentGroup>(sql).ToList();
await context.SaveChangesAsync();
Console.WriteLine();