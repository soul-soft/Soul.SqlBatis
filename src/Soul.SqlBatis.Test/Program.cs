using MySqlConnector;
using Soul.SqlBatis;
using Soul.SqlBatis.Test;

var context = new MyDbContext(new DbContextOptions
{
    ConnecionProvider = () => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test")
});
var student = new Student()
{
    FirstName = "Test",
    Name = "Test",
    CreationTime = DateTime.Now,
};
//context.Add(student);
context.OpenDbConnection();
var query = context.Students.GroupBy(x => x.FirstName).Clone();
var list = context.Students.GroupBy(x => x.FirstName)
    .Select(s => new
    {
        s.FirstName,
        Count = DbFunctions.Count(s.FirstName),
        Avg = DbFunctions.Avg(s.Id)
    }).ToList();

await context.SaveChangesAsync();
Console.WriteLine();