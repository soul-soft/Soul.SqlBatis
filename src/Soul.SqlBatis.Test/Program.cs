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
context.Delete(new Student 
{
	Id = 11,
	Name= "霄雲",
});
await context.SaveChangesAsync();
Console.WriteLine();