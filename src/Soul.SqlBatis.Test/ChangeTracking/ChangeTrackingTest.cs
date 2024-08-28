using Soul.SqlBatis.Entities;

namespace Soul.SqlBatis.Test.ChangeTracking
{
    [TestClass]
    public class ChangeTrackingTest
    {
        [TestMethod]
        public void TestAdd()
        {
            var context = DbContextFactory.CreateDbContext();
            var student = new Student { Name = "admin" };
            context.Add(student);
        }
    }
}
