using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace oracle4net
{
    class OracleDatabase
    {
        private OracleConnection con;
        private OracleConnectionStringBuilder str;
        private OracleCommand cmd;
        private OracleDataReader rdr;

        public OracleDatabase()
        {
            this.con = new OracleConnection();
            this.str = new OracleConnectionStringBuilder();
            this.cmd = new OracleCommand();
            Console.WriteLine("OracleDatabase created");
        }

        public void Connect()
        {
            Connect(ConnectionParams.UserID, ConnectionParams.Password, ConnectionParams.DataSource);
        }

        public void Connect(string UserID, string Password, string DataSource)
        {
            str.UserID = UserID;
            str.Password = Password;
            str.DataSource = DataSource;
            con.ConnectionString = str.ToString();
            con.Open();
            cmd.Connection = con;
            Console.WriteLine($"Connected to Oracle Database {con.ServerVersion}");
            Console.WriteLine($"Connected as {con.GetSchema()}@{con.DatabaseName}");
        }

        public bool IsConnected() => con.State == ConnectionState.Open;

        public void Disconnect()
        {
            string serverVersion = con.ServerVersion;
            con.Close();
            con.Dispose();
            Console.WriteLine($"Disconnected from Oracle Database {serverVersion}");
        }

        public int SQLCount(string table, string whereClause)
        {
            int count = 0;
            cmd.CommandText = "SELECT COUNT(*) FROM " + table + " WHERE " + whereClause;
            rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                count = rdr.GetInt32(0);
            }
            return count;
        }
    }
}
