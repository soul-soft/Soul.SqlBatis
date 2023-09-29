using Microsoft.EntityFrameworkCore;

namespace SqlBatis.Entityframework
{
    internal class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {
                
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>().ToTable(nameof(Student));        
        }
		
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
            base.OnConfiguring(optionsBuilder);
            this.Database.BeginTransaction();
            this.Add(new Student());
            this.Find<Student>();
            //this.Set<Student>().AsNoTracking;
		}

	}
}
