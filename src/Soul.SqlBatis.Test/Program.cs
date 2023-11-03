﻿using System.Text;
using Soul.SqlBatis;
using Soul.SqlBatis.Infrastructure;

var options = new DbContextOptionsBuilder()
    //.EnableQueryTracking()
    //.UseConnectionFactory(() => new MySqlConnector.MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test") ,DBMS.MSSQL)
    .UseConnectionFactory(() => new Microsoft.Data.SqlClient.SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=test;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"), DBMS.MSSQL)
    .Build();
TypeMapper.AddConverter<DateTime, string>(Converts.DatetimtToString);
using var context = new MyDbContext(options);
context.OpenDbConnection();
var student = context.Students.Where(a => a.Id == 3).Single();
var text = Encoding.UTF8.GetString(student.BinaryData);
var list1 = context.Students.ToList();
var list2 = context.Students.ToList();
int w = 2;
decimal a = (decimal)w;
Console.WriteLine();


Console.WriteLine();
