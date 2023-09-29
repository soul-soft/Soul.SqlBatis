using MySql.Data.MySqlClient;
using Soul.SqlBatis;
using Soul.SqlBatis.Entities;
using Soul.SqlBatis.Infrastructure;

var options = new DbContextOptionsBuilder()
	.AsTracking()
	.UseConnectionFactory(() => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test"))
	.Build();
var context = new MyDbContext(options);


var param1 = new DynamicParameters();
var sb1 = context.Students
	.Where(a => a.Id > 10)
	.Where(a => a.Id > 10)
	.Where(a => a.Id > 10)
	.Where(a => a.Id > 10)
	.Where(a => a.Id > 10)
	.Where(a => a.Id > 10)
	.Where(a => a.Id > 10)
	.Build(param1);
var view1 = $@"
SELECT
	student.Name,
	Math
FROM
	student
LEFT JOIN (
	SELECT
		StuId,
		SUM(Math) Math
	FROM 
		score
	GROUP
		StuId
) AS student_score.StuId = student.iD
{sb1.WhereSql}
";
Console.WriteLine(view1);
var list1 = context.FromSql<StudentByScore>(view1,param1).ToList();
var whereSql1 = sb1.WhereSql;

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