using Soul.SqlBatis.Entities;

namespace Soul.SqlBatis.Test.Database
{
    [TestClass]
    public class DbContextCommandTest
    {
        [TestMethod("测试IN查询")]
        public void TestIn1()
        {
            var param = new Dictionary<string, object>();
            param.Add("id", new int[] { 1, 2, 3 });
            var count = DbContextFactory.CreateDbContext().Command.Execute("delete from student where id in @id", param);
            Assert.AreEqual(3, count);
        }

        [TestMethod("测试IN查询")]
        public void TestIn2()
        {
            var param = new Dictionary<string, object>();
            param.Add("id", new int[0]);
            var count = DbContextFactory.CreateDbContext().Command.Execute("delete from student where id in @id", param);
            Assert.AreEqual(0, count);
        }

        [TestMethod("测试Query查询")]
        public void TestQuery1()
        {
            var list = DbContextFactory.CreateDbContext().Command.Query<Student>("select * from student");
        }
    }
}
