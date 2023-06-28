// See https://aka.ms/new-console-template for more information


using Soul.SqlBatis;
var req = new
{
    Age = (int?)50
};
var sb = new SqlBuilder();
sb = sb.From("students join schools on students.sch_id = schools.id")
    .Where("age > @Age", req.Age != null)
    .OrderBy("students.age desc");

var querySql = sb.Page(1,20).Select("students.id,students.age,schools.name");
var countSql = sb.Page(1, 20).Count();
Console.WriteLine(querySql);
Console.WriteLine(countSql);
