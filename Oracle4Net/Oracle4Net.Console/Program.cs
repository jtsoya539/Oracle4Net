using System;

namespace Oracle4Net.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Hello World!");
            OracleDatabase db = new OracleDatabase();
            db.Connect("risk", "ruffus", "RISK");
            System.Console.WriteLine(db.IsConnected());
            int res = db.ExecuteSQLCount("countries", "region_id = 'SOUTH'");
            db.ExecuteSQL("select e.employee_id, e.first_name, e.last_name, e.email from employees e");
            db.Disconnect();
            System.Console.WriteLine(db.IsConnected());
        }
    }
}
