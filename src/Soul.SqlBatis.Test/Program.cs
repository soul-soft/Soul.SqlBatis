// See https://aka.ms/new-console-template for more information


using Soul.SqlBatis;
var req = new
{
    Age = (int?)50
};
var query = new SqlBuilder();
query = query.From("students join schools on students.sch_id = schools.id")
    .Where("age > @Age", req.Age != null)
    .OrderBy("students.age desc")
    .Page(1, 20);

var querySql = query.Select();
var countSql = query.Count();
Console.WriteLine(querySql);
Console.WriteLine(countSql);

var update = new SqlBuilder()
    .From("student")
    .Set("age = @Age")
    .Set("name = @Name")
    .Where("id = 1")
    .Update();
Console.WriteLine(update);

var delete = new SqlBuilder()
    .From("student")
    .Where("id = 1")
    .Delete();
Console.WriteLine(delete);
