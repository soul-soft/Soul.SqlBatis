using MySqlConnector;
using Soul.SqlBatis;
using Soul.SqlBatis.Test;

var context = new MyDbContext(new DbContextOptions
{
    ConnecionProvider = () => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test")
});
var context1 = new MyDbContext(new DbContextOptions
{
	ConnecionProvider = () => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test")
});

var list = new int[] { };
var req = new { Age = 50 };
var names = new string[]
{
    "78",
    "74"
};
var sql = context.Students
    .FromSql("students")
    .Where(a => a.Id > 0 && a.Name != "fa")
    .Where(a => DbOperations.Between(a.Id, 1, 2))
    .Where(a => SqlFunctions.Now() > a.CreationTime)
    .ToList();
Console.WriteLine();
