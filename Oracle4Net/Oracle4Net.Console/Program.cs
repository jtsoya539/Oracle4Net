using System;

namespace Oracle4Net.Console
{
    class Program
    {


        static void Main(string[] args)
        {
            const string dataSource = "(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=JAVIER-HP)(PORT=1521))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=XEPDB1)))";
            OracleDatabase db = new OracleDatabase();
            System.Console.WriteLine(db.IsConnected());
            try
            {
                db.Connect("risk", "ruffus", dataSource);
                db.ExecuteSQLCount("t_usuarios", "1=1");
                db.Disconnect();
            }
            catch (OracleDatabaseException)
            {
                System.Console.WriteLine("Hubo un error");
            }
        }
    }
}
