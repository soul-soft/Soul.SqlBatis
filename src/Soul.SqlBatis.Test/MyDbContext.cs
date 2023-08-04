﻿using Soul.SqlBatis.Infrastructure;

namespace Soul.SqlBatis.Test
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<Student> Students => Set<Student>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Ignore<Student>();
            builder.Entity<Student>().ToTable(nameof(Student));
            builder.Entity<Student>().ToView(nameof(Student));
            builder.Entity<Student>().HasKey(a => a.Id);
            builder.Entity<Student>().Ignore(a => a.FirstName);
        }
    }
}
