using Microsoft.EntityFrameworkCore;

namespace SqlBatis.Entityframework
{
    internal class AppDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable(nameof(Student));
        }
		
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
            this.Database.BeginTransaction();
		}
	}
}
