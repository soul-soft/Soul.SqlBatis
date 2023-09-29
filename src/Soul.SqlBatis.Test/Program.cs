using MySql.Data.MySqlClient;
using Soul.SqlBatis;
using Soul.SqlBatis.Infrastructure;

var options = new DbContextOptionsBuilder()
	.AsTracking()
	.UseConnectionFactory(() => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test"))
	.Build();
var context = new MyDbContext(options);


var sb = new SqlBuilder();
//分组之前过率
sb.Where("id > @Id");
var view = $@"
SELECT
    group_concat(id) Ids,
    Name,
    Count(*) count
FROM
    student
{sb.WhereSql}
GROUP BY
    student.Name
";
var param = new DynamicParameters();
param.Add("Id", 18968);
//视图过滤
var list = context.FromSql<StudentByName>(view, param)
	.Where(a => a.Count > 1)
	.ToList();
Console.WriteLine();