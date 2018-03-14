using System;
using Xunit;
using oracle4net;

namespace oracle4net.Tests
{
    public class OracleDatabaseTest
    {
        // Arrange
        // Act
        // Assert

        OracleDatabase db;

        public OracleDatabaseTest()
        {
            db = new OracleDatabase();
            db.Connect("ne", "ruffus", "rama2");
        }

        [Fact]
        public void ExecuteSQLCountWithCorrectArguments()
        {
            Assert.Equal(15, db.ExecuteSQLCount("t_clubes", "1=1"));
        }

        [Fact]
        public void ExecuteStoredFunctionVarchar2WithCorrectArgument()
        {
            Assert.Equal("CLUB CERRO PORTEÑO", db.ExecuteStoredFunctionVarchar2("f_nombre_club('CER')"));
        }

    }
}