// See https://aka.ms/new-console-template for more information


using Soul.SqlBatis;
var req = new
{
    Age = (int?)50
};

var query = new SqlBuilder();
var sb = query
    .From("students")
    .Where("age > @Age", req.Age != null)
    .OrderBy("students.age desc")
    .GroupBy("students.name");

var count = sb.Count();
Console.WriteLine(count);
