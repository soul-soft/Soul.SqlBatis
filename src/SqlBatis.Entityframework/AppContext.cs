using Microsoft.EntityFrameworkCore;

namespace SqlBatis.Entityframework
{
    internal class AppDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable(nameof(Student));
            this.Entry(new Student()).State;
        }
    }
}
