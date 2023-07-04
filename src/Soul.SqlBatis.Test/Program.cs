// See https://aka.ms/new-console-template for more information


using Soul.SqlBatis;
var req = new
{
    Age = (int?)50
};

var query = new SqlBuilder();
var sb = query.Page(1, 20)
    .Join("LEFT JOIN schools ON schools.sc_id = students.id")
    .Join("LEFT JOIN schools ON schools.sc_id = students.id")
    .Build("SELECT * FROM students /**JOIN**/ /**LIMIT**/");

var count = sb.Count();
Console.WriteLine(count);
