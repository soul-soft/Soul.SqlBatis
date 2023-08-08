﻿using MySqlConnector;
using Soul.SqlBatis;
using Soul.SqlBatis.Test;

var context = new MyDbContext(new DbContextOptions
{
    ConnecionProvider = () => new MySqlConnection("Server=localhost;Port=3306;User ID=root;Password=1024;Database=test")
});
var student = new Student()
{
    FirstName = "Test",
    Name = "Test",
    CreationTime = DateTime.Now,
};
//context.Add(student);
context.OpenDbConnection();
var list = context.Students
    .Where(a => a.Id > 0)
    .Skip(1)
    .ToList();
await context.SaveChangesAsync();
Console.WriteLine();