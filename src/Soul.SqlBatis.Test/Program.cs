﻿using System.Data.Common;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using Soul.SqlBatis;
using Soul.SqlBatis.Infrastructure;

var context = new MyDbContext(configure =>
{
    configure.UseLoggerFactory(LoggerFactory.Create(logging =>
    {
        logging.AddConsole();
    }));
    configure.UseMySql(new MySqlConnection("Server=192.168.0.155;Port=3306;User ID=root;Password=1024;Database=test"));
});
var list = new List<int>()
{

};
var students = context.Students
    .Where(a => a.CreationTime > DateTime.Now)
    .ToList();
var row = await context.SaveChangesAsync();
Console.WriteLine();