using System;
using Xunit;

namespace Oracle4Net.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            OracleDatabase db = new OracleDatabase();
            db.Connect("risk", "ruffus", "RISK");
        }
    }
}
