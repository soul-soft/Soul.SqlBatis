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
            var list = context.Set<Student>().Where(a => DbOps.InSet(a.Name, "0,fa")).ToList();
            context.SaveChanges();
        }
    }
}


