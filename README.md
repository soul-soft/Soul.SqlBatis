# Soul.SqlBatis

- Flexible and simple configuration, supports LINQ+SQL, and entity change tracking.
- Supports customization; refer to the source code for details.
- The source code is concise and easy to understand, with around 4000 lines of code (algorithms + data structures).
- The author retains all final rights, and the open-source license is MIT.Configuring DbContext

```C#
var context = new MyDbContext(configure =>
{
    //logger
    configure.UseLogger((sql, param) =>
    {
        Console.WriteLine(sql);
        Debug.WriteLine(sql);
    });
    //query tracking
    configure.UseQueryTracking();
    //connect
    configure.UseConnection(new MySqlConnection("Server=127.0.0.1;User ID=root;Password=1024;Database=test"));
});
```

## Configuring Model

* It is recommended to provide a parameterless constructor.

* By default, a field named "Id" is considered the primary key and auto-increment column.

* Use [NotIdentity] to remove the auto-increment feature of a column named "Id".

* For customization, implement the IModel interface.

```C#
public class Student : Entity
{
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public string Name { get; set; }

    [Column("address")]
    public Address Address { get; set; }

    [Column("create_time")]
    public DateTime? CreateTime { get; set; }
}

public class Address(string cityName, string areaName)
{
    public string CityName { get; } = cityName;
    public string AreaName { get; } = areaName;
}
```

## Query Syntax

### List

```C#
var list = context.Set<Student>().ToList();
var (list, total) = context.Set<Student>().ToPageList(1, 10);
```

### Statistics

```C#
var count = context.Set<Student>().Count();
var sum = context.Set<Student>().Sum(a => a.Id);
var min = context.Set<Student>().Min(a => a.Id);
var max = context.Set<Student>().Max(a => a.Id);
var has = context.Set<Student>().Any(a => a.Id > 10);
var avg = context.Set<Student>().Average(a => a.Id);
```

### IN Query

```C#
var ids = new List<int>() {1, 2, 3};
var list = context.Set<Student>()
    .Where(a => ids.Contains(a.Id))
    .ToList();
var list = context.Set<Student>()
    .Where(a => DbOps.In(a.Id, ids))
    .ToList();
var list = context.Set<Student>()
    .Where(a => DbOps.In(a.Id, 1, 2, 3))
    .ToList();
var list = context.Set<Student>()
    .Where(a => DbOps.InSet(a.Id, "1,2,3"))
    .ToList();
var list = context.Set<Student>()
    .Where(a => DbOps.InSub(a.Id, "SELECT stu_id FROM grades WHERE level = 1"))
    .ToList();
```

### Parameterized Query

```C#
var parameter = new DynamicParameters();
parameter.Add("Level", 1);
var list = context.Set<Student>(parameter)
    .Where(a => DbOps.InSub(a.Id, "SELECT stu_id FROM grades WHERE level = @Level"))
    .ToList();
```

### Query Reuse

```C#
// Function 1: Can be encapsulated into a function for reuse
var parameter = new DynamicParameters();
parameter.Add("Level", 1);
var query = context.Set<Student>(parameter)
    .Where(a => DbOps.InSub(a.Id, "SELECT stu_id FROM grades WHERE level = @Level"))
    .OrderBy(a => a.Id);

// Function 2: List query
var (list, total) = query.OrderBy(a => a.Id).ToPageResult(1, 20);

// Function 3: Statistics query
var (sb, parameters) = query.As("stu").Build();
var view = $@"
SELECT
    stu.id,
    stu.name,
    math_avg
FROM
    student AS stu
LEFT JOIN (
    SELECT
        stu_id,
        AVG(math) math_avg
    FROM
        student_score
    GROUP BY
        stu_id
) AS sc ON stu.id = sc.stu_id
{sb.WhereSql}
{sb.OrderSql}
";
var list = context.Command.Query<StudentAvgDto>(view, parameters);
```

### SqlBuilder

```C#
// Query parameters
var req  = new 
{
    Level = (int?)1,
    StartTime = (DateTime?)DateTime.Now,
    EndTime = (DateTime?)null
};

// Dynamic parameters
var parameter = new DynamicParameters();
parameter.Add(req);

// Query body
var sb = new SqlBuilder();
sb.Where("math_avg > 89", req.Level != null);
sb.Order("math_avg_ DESC");
sb.Page(1, 10);
// Build dynamic score query
var sbScore = new SqlBuilder();
sbScore.Where("create_time >= @StartTime" , req.StartTime != null);
sbScore.Where("create_time <= @EndTime", req.EndTime != null);
var view = $@"
SELECT
    stu.id,
    stu.name,
    math_avg
FROM
    student AS stu
LEFT JOIN (
    SELECT
        stu_id,
        AVG(math) math_avg
    FROM
        student_score
    {sbScore.WhereSql}        
    GROUP BY
        stu_id
) AS sc ON stu.id = sc.stu_id
{sb.WhereSql}
{sb.OrderSql}
{sb.LimitSql}
/**counter sql** /
;SELECT 
    COUNT(**)
FROM
    student AS stu
LEFT JOIN (
    SELECT
        stu_id,
        AVG(math) math_avg
    FROM
        student_score
    {sbScore.WhereSql}        
    GROUP BY
        stu_id
) AS sc ON stu.id = sc.stu_id
{sb.WhereSql}
";
// Initiate query
using(var mutil = context.Command.QueryMultiple(view, parameters))
{
    var list = mutil.Read<StudentAvgDto>();
    var total = mutil.ReadFirst<int>();
}
```

## Update Query

```C#
var f = context.Set<Student>()
    .Where(a => a.Id == 1)
    .ExecuteUpdate(setters => setters
        .SetProperty(a => a.Name, "zs")
        .SetProperty(a => a.State, a => a.State + 1));
```

## Delete Query

```C#
var f = context.Set<Student>()
    .Where(a => a.Id == 1)
    .ExecuteDelete();
```

## Custom Functions

1. Custom functions are used to map to database functions.
2. Custom functions must be defined in a static class and only need to be declared without implementation.
3. The class or the function itself should have the [DbFunction] attribute.
4. Supports parameter templating with Format. In Format, braces are used as parameter placeholders.
5. The function name defaults to the final database function, but you can specify with [DbFunction(Name = "COUNT")].

```C#
[DbFunction]
public static class DbFunc
{
    [DbFunction(Format = "*")]
    public static int Count()
    {
        throw new NotImplementedException();
    }

    [DbFunction(Name = "COUNT", Format = "DISTINCT {column}")]
    public static int DistictCount<T>(T column)
    {
        throw new NotImplementedException();
    }

    public static int Count<T>(T column)
    {
        throw new NotImplementedException();
    }

    public static T IF<T>(bool column, T value1, T value2)
    {
        throw new NotImplementedException();
    }
}

var list = context.Set<Student>()
    .Select(s => DbFunc.IF(s.State > 10, "A", "S"))
    .ToList();
```

## Custom Type Mapping

### Method 1

1. The UseTypeMapper method is used to handle member property types, i.e., the UseTypeMapper method returns a bool indicating handling for properties of type bool.
2. No need to handle Nullable types; the framework will automatically manage it.
3. The advantage of this method is its simplicity, but it lacks flexibility and context information.
4. This method has lower priority than the factory method but higher than the default method.

```C#
var context = new MyDbContext(configure =>
{
    configure.UseEntityMapper(configureOptions =>
    {
        // Handle bool type
        configureOptions.UseTypeMapper((record, i) =>
        {
            var result = record.GetInt16(i);
            return result == 0 ? false : true;
        });
        // Handle string
        configureOptions.UseTypeMapper((record, i) =>
        {
            return record.GetString(i);
            throw new InvalidOperationException();
        });
        // Handle timeSpan
        configureOptions.UseTypeMapper((record, i) =>
        {
            if (record is MySqlDataReader reader)
            {
                return reader.GetTimeSpan(i);
            }
            throw new InvalidOperationException();
        });
        // Handle bytes
        configureOptions.UseTypeMapper((record, i) =>
        {
            var buffer = new byte[1024];
            var count = record.GetBytes(i, 0, buffer, 0, buffer.Length);
            var span = new Span<byte>(buffer, 0, (int)count);
            return span.ToArray();
        });
    });
    configure.UseConnection(new MySqlConnection("Server=127.0.0.1;User ID=root;Password=1024;Database=test"));
});
```

### Method 2

1. You can configure using a factory method, which is more flexible.
2. Methods 1 and 2 can be used simultaneously, but method 2 has higher priority than method 1.
3. The returned TypeMapper must be a static function and cannot be a generic method; if it is, it should be made non-generic.
4. The method should have only two parameters, with the first being of type IDataRecord and the second being int.
5. Avoid writing unrelated code in the mapper method to prevent performance issues.
6. The return type of the function must match the MemberType.

```C#
public class TypeMapperFactory : ITypeMapperFactory
{
    // Must be a static function
    public static T StringToJson<T>(IDataRecord record, int i)
    {
        return JsonSerializer.Deserialize<T>(record.GetString(i), new JsonSerializerOptions 
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        })
        ?? throw new NullReferenceException();
    }

    public static T StringToString(IDataRecord record, int i)
    {
        return record.GetString(i);
    }

    public static T BytesToString(IDataRecord record, int i)
    {
        using(var ms = new MemoryStream())
        {
            while(true)
            {
                var buffer = new byte[1024 * 16];
                var count = (int)record.GetBytes(i, 0, buffer, 0, buffer.Length);
                if(count > 0)
                    ms.Write(buffer, 0, count);
                else
                    break;
            }
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }

    public MethodInfo? GetTypeMapper(TypeMapperContext context)
    {
        // Distinguish which mapper to use; if DB does not support JSON type, you can add annotations to the field or type for judgment
        if ("json".Equals(context.FieldTypeName, StringComparison.OrdinalIgnoreCase) && context.FieldType == typeof(string) && context.MemberType != typeof(string))
        {
            return GetType().GetMethod(nameof(StringToJson))!.MakeGenericMethod(context.MemberType);
        }
        
        // String to string
        if(context.FieldType == typeof(string) && context.MemberType == typeof(string))
            return GetType().GetMethod(nameof(StringToString));
        
        // Byte[] to string
        if(context.FieldType == typeof(byte[]) && context.MemberType == typeof(string))
            return GetType().GetMethod(nameof(BytesToString));
        return null;
    }
}
// Apply type mapping factory
var context = new MyDbContext(configure =>
{
    configure.UseEntityMapper(configureOptions =>
    {
          configureOptions.TypeMapperFactory = new TypeMapperFactory();
    });
    configure.UseConnection(new MySqlConnection("Server=127.0.0.1;User ID=root;Password=1024;Database=test"));
});
```