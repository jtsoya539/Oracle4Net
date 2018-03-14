using System;
using Xunit;
using oracle4net;

namespace oracle4net.Tests
{
    public class OracleDatabaseTest
    {
        OracleDatabase db;

        public OracleDatabaseTest()
        {
            db = new OracleDatabase();
            //db.Connect();
        }

        [Fact]
        public void Test1()
        {
            Assert.Equal(14, db.ExecuteSQLCount("emp", "columna = 2"));
        }
    }
}