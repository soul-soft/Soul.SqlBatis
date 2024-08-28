namespace Soul.SqlBatis.Test.Database
{
    [TestClass]
    public class SqlBuilderTest
    {
        [TestMethod("测试WhereSql")]
        public void TestWhere1()
        {
            var sb = new SqlBuilder();
            sb.Where("a = 1");
            sb.Where("a = 3");
            sb.Where("a = 2", false);
            Assert.IsTrue("WHERE a = 1 AND a = 3".Equals(sb.WhereSql));
        }

        [TestMethod("测试WhereSql")]
        public void TestWhere12()
        {
            var sb = new SqlBuilder();
            Assert.IsTrue("".Equals(sb.WhereSql));
        }

        [TestMethod("测试OrderSql")]
        public void TestOrder1()
        {
            var sb = new SqlBuilder();
            sb.OrderBy("a desc");
            sb.OrderBy("b asc");
            sb.OrderBy("c asc", false);
            Assert.IsTrue("ORDER BY a desc, b asc".Equals(sb.OrderSql));
        }

        [TestMethod("测试OrderSql")]
        public void TestOrder2()
        {
            var sb = new SqlBuilder();
            Assert.IsTrue("".Equals(sb.OrderSql));
        }

        [TestMethod("测试Clone")]
        public void TestMethod3()
        {
            var sb1 = new SqlBuilder();
            sb1.OrderBy("a desc");
            sb1.Where("a = 1");
            var sb2 = sb1.Clone();
            Assert.IsTrue(sb1.WhereSql == sb2.WhereSql && sb1.OrderSql == sb2.OrderSql);
        }
    }
}
