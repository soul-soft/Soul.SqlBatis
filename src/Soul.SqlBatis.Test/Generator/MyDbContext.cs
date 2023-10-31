

using System;
using Soul.SqlBatis;
using Soul.SqlBatis.Entities;

namespace Soul.SqlBatis.Infrastructure
{
	public partial class MyDbContext : DbContext
	{
		/// <summary>
		/// 
		/// </summary>
		public DbSet<Student> Students => Set<Student>();
		/// <summary>
		/// 
		/// </summary>
		public DbSet<StudentScore> StudentScores => Set<StudentScore>();
	}
}

