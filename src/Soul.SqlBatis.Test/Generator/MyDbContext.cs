

using System;
using Soul.SqlBatis;
using Soul.SqlBatis.Entities;

namespace Soul.SqlBatis.Infrastructure
{
	public partial class MyDbContext : DbContext
	{
		/// <summary>
		/// VIEW
		/// </summary>
		public DbSet<Ff> Ffs => Set<Ff>();
		/// <summary>
		/// 
		/// </summary>
		public DbSet<Students> Studentss => Set<Students>();
		/// <summary>
		/// View 'test.v_order' references invalid table(s) or column(s) or function(s) or definer/invoker of view lack rights to use them
		/// </summary>
		public DbSet<VOrder> VOrders => Set<VOrder>();
	}
}

