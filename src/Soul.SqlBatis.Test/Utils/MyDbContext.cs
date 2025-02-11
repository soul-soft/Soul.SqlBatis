

using System;
using Soul.SqlBatis;
using Soul.SqlBatis.Entities;

namespace Soul.SqlBatis.Infrastructure
{
	public partial class MyDbContext : DbContext
	{
		/// <summary>
		/// 创收单主表
		/// </summary>
		public DbSet<CreateIncome> CreateIncomes => Set<CreateIncome>();
		/// <summary>
		/// 开票主表
		/// </summary>
		public DbSet<Invoice> Invoices => Set<Invoice>();
		/// <summary>
		/// 开票回款
		/// </summary>
		public DbSet<InvoiceBack> InvoiceBacks => Set<InvoiceBack>();
		/// <summary>
		/// 本系统组织架构-公司部门
		/// </summary>
		public DbSet<Organization> Organizations => Set<Organization>();
		/// <summary>
		/// 创收单-产值分配-部门
		/// </summary>
		public DbSet<OutputValueDep> OutputValueDeps => Set<OutputValueDep>();
		/// <summary>
		/// 创收单-产值分配-人员
		/// </summary>
		public DbSet<OutputValuePer> OutputValuePers => Set<OutputValuePer>();
		/// <summary>
		/// 项目信息
		/// </summary>
		public DbSet<ProjectInfo> ProjectInfos => Set<ProjectInfo>();
		/// <summary>
		/// 结算咨询-基本信息
		/// </summary>
		public DbSet<ProjectJsInfo> ProjectJsInfos => Set<ProjectJsInfo>();
		/// <summary>
		/// 结算咨询-项目成员
		/// </summary>
		public DbSet<ProjectJsUser> ProjectJsUsers => Set<ProjectJsUser>();
		/// <summary>
		/// 项目成员
		/// </summary>
		public DbSet<ProjectUser> ProjectUsers => Set<ProjectUser>();
		/// <summary>
		/// VIEW
		/// </summary>
		public DbSet<VProjectInfoUnion> VProjectInfoUnions => Set<VProjectInfoUnion>();
		/// <summary>
		/// VIEW
		/// </summary>
		public DbSet<VProjectUserUnion> VProjectUserUnions => Set<VProjectUserUnion>();
		/// <summary>
		/// View 'test.v_student' references invalid table(s) or column(s) or function(s) or definer/invoker of view lack rights to use them
		/// </summary>
		public DbSet<VStudent> VStudents => Set<VStudent>();
	}
}

