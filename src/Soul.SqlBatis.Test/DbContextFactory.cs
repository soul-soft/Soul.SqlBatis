using MySqlConnector;
using System.Diagnostics;

namespace Soul.SqlBatis.Test
{
    public class DbContextFactory
    {
        public static DbContext CreateDbContext()
        {
            return new MyDbContext(configure =>
            {
                configure.UseLogger((sql, param) =>
                {
                    Console.WriteLine(sql);
                    Debug.WriteLine(sql);
                });
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
                    //注册映射工厂
                    configureOptions.TypeMapperFactory = new TypeMapperFactory();
                });
                configure.UseQueryTracking();
                configure.UseConnection(new MySqlConnection("Server=127.0.0.1;User ID=root;Password=1024;Database=test"));
            });
        }
    }

}
