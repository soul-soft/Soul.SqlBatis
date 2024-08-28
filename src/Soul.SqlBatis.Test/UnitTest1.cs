using Soul.SqlBatis.Entities;

namespace Soul.SqlBatis.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var context = DbContextFactory.CreateDbContext();
            var ids = new int?[] { 1 };
            var list = context.Set<MysqlDataTypes>()
                .Where(a => a.VarcharCol.EndsWith("v"))
                .ToList();
            context.SaveChanges();
        }
    }
}


