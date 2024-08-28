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
                .Where(a => a.Id == 20)
                .ExecuteUpdate(setters => setters
                    .SetProperty(a => a.IntCol, 20)
                );
            context.SaveChanges();
        }
    }
}


