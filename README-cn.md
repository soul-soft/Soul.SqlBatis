# Soul.SqlBatis

* 配置灵活简单，支持linq+sql，支持实体更改跟踪

* 支持个性化，具体参考源码

* 源码简短易懂只有4000左右行代码（算法+数据结构）

* 转载格式必须注明：【作者：花间岛，原包：Soul.SqlBatis】

* 作者保留最终所有权，开源协议采用MIT

## 配置DbContext

```` C#
var context = new MyDbContext(configure =>
{
    //设置日志
    configure.UseLogger((sql, param) =>
    {
        Console.WriteLine(sql);
        Debug.WriteLine(sql);
    });
    //启用查询跟踪
    configure.UseQueryTracking();
    //设置连接对象
    configure.UseConnection(new MySqlConnection("Server=127.0.0.1;User ID=root;Password=1024;Database=test"));
});
````

## 配置Model

1. 建议一定要提供一个无参构造器
2. 默认认为名为Id的字段为主键和自增列
3. 使用[NotIdentity]可以移除名为Id列的自增特征
4. 如需个性化，请实现IModel接口


```` C#

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
````

## 查询语法

### 列表

```` C#
var list = context.Set<Student>().ToList();
var (list, total) = context.Set<Student>().ToPageList(1, 10);
````

### 统计

```` C#
var count = context.Set<Student>().Count();
var sum = context.Set<Student>().Sum(a => a.Id);
var min = context.Set<Student>().Min(a => a.Id);
var max = context.Set<Student>().Max(a => a.Id);
var has = context.Set<Student>().Any(a => a.Id > 10);
var avg = context.Set<Student>().Average(a => a.Id);
````

### IN查询

```` C#
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
````

### 参数化查询

```` C#
var parameter = new DynamicParameters();
parameter.Add("Level", 1);
var list = context.Set<Student>(parameter)
    .Where(a => DbOps.InSub(a.Id, "SELECT stu_id FROM grades WHERE level = @Level"))
    .ToList();
````

### 查询复用

```` C#
//函数1：可以封装到一个函数里，用于复用
var parameter = new DynamicParameters();
parameter.Add("Level", 1);
var query = context.Set<Student>(parameter)
    .Where(a => DbOps.InSub(a.Id, "SELECT stu_id FROM grades WHERE level = @Level"))
    .OrderBy(a => a.Id);

//函数2：列表查询
var (list, total) = query.OrderBy(a => a.Id).ToPageResult(1, 20);

//函数3：统计查询
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
````

### SqlBuilder

```` C#
//查询参数
var req  = new 
{
    Level = (int?)1,
    StartTime = (DateTime?)DateTime.Now,
    EndTime = (DateTime?)null
};

//动态参数
var parameter = new DynamicParameters();
parameter.Add(req);

//查询主体
var sb = new SqlBuilder();
sb.Where("math_avg > 89", req.Level != null);
sb.Order("math_avg_ DESC");
sb.Page(1, 10);
//构建成绩动态查询
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
/**计数语句**/
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
//发起查询
using(var mutil = context.Command.QueryMultiple(view, parameters))
{
    var list = mutil.Read<StudentAvgDto>();
    var total = mutil.ReadFirst<int>();
}
````


## 更新查询

```` C#
 var f = context.Set<Student>()
    .Where(a => a.Id == 1)
    .ExecuteUpdate(setters => setters
        .SetProperty(a => a.Name, "zs")
        .SetProperty(a => a.State, a => a.State + 1));
````

## 删除查询

```` C#
 var f = context.Set<Student>()
    .Where(a => a.Id == 1)
    .ExecuteDelete();
````

## 自定义函数

1. 自定义函数用于对数据库函数进行映射
2. 自定义函数必须定义在静态类中，只需声明无需实现
3. 函数定义的类或者函数自身带有[DbFunction]特性
4. 支持参数模板化Format。Format里花括号里是参数占位标记
5. 函数名默认为最终的数据库函数，可以通过[DbFunction(Name = "COUNT")]指定

```` C#
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
````

## 自定义类型映射

### 方式一

1. 通过UseTypeMapper方式是通过成员的属性类型来查找的，即UseTypeMapper方法返回bool表示用于处理属性类型是bool的情况
2. 无需处理Nullable情况，框架内部会自动处理
3. 方式的优点是配置简单，确定是不够灵活，缺失上下文信息
4. 该方式优先级低于工程模式，高于默认的模式

```` C#
var context = new MyDbContext(configure =>
{
    configure.UseEntityMapper(configureOptions =>
    {
        //处理bool类型
        configureOptions.UseTypeMapper((record, i) =>
        {
            var result = record.GetInt16(i);
            return result == 0 ? false : true;
        });
        //处理string
        configureOptions.UseTypeMapper((record, i) =>
        {
            return record.GetString(i);
            throw new InvalidOperationException();
        });
        //处理timeSpan
        configureOptions.UseTypeMapper((record, i) =>
        {
            if (record is MySqlDataReader reader)
            {
                return reader.GetTimeSpan(i);
            }
            throw new InvalidOperationException();
        });
        //处理bytes
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
````

### 方式二

1. 可以通过工程模式来进行配置，工厂模式更加的灵活
2. 方式一和方式二可以同时使用，但是方式二的优先级高于方式一
3. 返回的TypeMapper必须是一个静态函数，且不能是泛型方法，如果是泛型方法应该make成非泛型方法
4. 参数的个数只有两个且第一个的类型必须是IDataRecord，第二必须是int
5. 不要在映射器方法内写无关代码来影响性能
6. 函数的返回类型，必须要和MemberType一致

```` C#
public class TypeMapperFactory : ITypeMapperFactory
{
    //必须是静态函数
    public static T StringToJson<T>(IDataRecord record, int i)
    {
        return JsonSerializer.Deserialize<T>(record.GetString(i),new JsonSerializerOptions 
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
                var buffer = new int[1024 * 16];
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
        //通过这个来区分使用那个映射器，如果DB不支持json类型，那么可以给字段添加注解，或者给类型添加注解，用于判断
        if ("json".Equals(context.FieldTypeName, StringComparison.OrdinalIgnoreCase) && context.FieldType == typeof(string) && context.MemberType != typeof(string))
        {
            return GetType().GetMethod(nameof(StringToJson))!.MakeGenericMethod(context.MemberType);
        }
        
        //string to string
        if(context.FieldType == typeof(string) && context.MemberType == typeof(string))
            return GetType().GetMethod(nameof(StringToString));
        
        //byte[] to string
        if(context.FieldType == typeof(byte[]) && context.MemberType == typeof(string))
            return GetType().GetMethod(nameof(BytesToString));
        return null;
    }
}
//应用类型映射工厂
var context = new MyDbContext(configure =>
{
    configure.UseEntityMapper(configureOptions =>
    {
          configureOptions.TypeMapperFactory = new TypeMapperFactory();
    });
    configure.UseConnection(new MySqlConnection("Server=127.0.0.1;User ID=root;Password=1024;Database=test"));
});
````