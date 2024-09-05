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
            var entity1 = context.Set<MysqlDataTypes>().Where(a=>a.Id == 1).First();
            var entity2 = context.Set<MysqlDataTypes>().Where(a => a.Id == 1).First();
            var flag = ReferenceEquals(entity1, entity2);
            context.SaveChanges();
        }
    }
}


