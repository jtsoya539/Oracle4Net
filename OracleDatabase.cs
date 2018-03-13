using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace oracle4net
{
    class OracleDatabase
    {
        private OracleConnection con;
        private OracleConnectionStringBuilder str;

        public OracleDatabase()
        {
            this.con = new OracleConnection();
            this.str = new OracleConnectionStringBuilder();
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
    }
}
