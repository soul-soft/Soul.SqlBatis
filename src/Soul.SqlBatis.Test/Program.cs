// See https://aka.ms/new-console-template for more information


using Soul.SqlBatis;
var req = new
{
    Age = (int?)50
};

var query = new SqlBuilder();
var sb = query.Page(1, 20).Build("SELECT * FROM STUDENTS /**LIMIT**/");

var count = sb.Count();
Console.WriteLine(count);
