using Soul.SqlBatis.Entities;

namespace Soul.SqlBatis.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using var context = DbContextFactory.CreateDbContext();
            var list = context.Set<Student>().Where(a => a.Name != null).ToList();
            context.SaveChanges();
        }
    }
}


