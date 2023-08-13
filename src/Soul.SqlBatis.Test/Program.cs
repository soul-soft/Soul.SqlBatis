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

var students = context.Query<Student>("select Name from students where id in @id",new {id = new int[] { 1,2,3} });

var row = await context.SaveChangesAsync();
Console.WriteLine();