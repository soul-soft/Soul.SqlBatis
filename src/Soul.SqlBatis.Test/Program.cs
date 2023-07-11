// See https://aka.ms/new-console-template for more information


using Soul.SqlBatis;
var req = new
{
    Age = (int?)50
};

var query = new SqlBuilder();
var sb = query.Where("Id = @Id",false);
var wh = sb.Build("/**WHERE**/");
var count = sb.Count();
Console.WriteLine(count);
