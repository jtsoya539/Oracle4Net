using System;
using System.Configuration;

namespace Oracle4Net.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["RISK"].ConnectionString;
            System.Console.WriteLine("connectionString: " + connectionString);
            OracleDatabase db = new OracleDatabase();
            db.Connect(connectionString);
            System.Console.WriteLine(db.IsConnected());
            int res = db.ExecuteSQLCount("countries", "region_id = 'SOUTH'");
            db.ExecuteSQL("select e.employee_id, e.first_name, e.last_name, e.email from employees e");
            db.Disconnect();
            System.Console.WriteLine(db.IsConnected());
        }
    }
}
