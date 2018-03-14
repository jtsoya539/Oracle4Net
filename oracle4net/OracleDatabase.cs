using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace oracle4net
{
    public class OracleDatabase
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
            try
            {
                con.Open();
                cmd.Connection = con;
                Console.WriteLine($"Connected to Oracle Database {con.ServerVersion}");
                Console.WriteLine($"Connected as {con.GetSchema()}@{con.DatabaseName}");
            }
            catch (OracleException oex)
            {
                Console.WriteLine(oex.Message);
            }
        }

        public bool IsConnected() => con.State == ConnectionState.Open;

        public void Disconnect()
        {
            string serverVersion = con.ServerVersion;
            con.Close();
            con.Dispose();
            Console.WriteLine($"Disconnected from Oracle Database {serverVersion}");
        }

        public int ExecuteSQLCount(string table, string whereClause)
        {
            if (!this.IsConnected())
            {
                throw new Exception("No connection to Oracle Database");
            }
            int count = 0;
            cmd.CommandText = "SELECT COUNT(*) FROM " + table + " WHERE " + whereClause;
            try
            {
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (OracleException oex)
            {
                Console.WriteLine(oex.Message);
            }
            return count;
        }

        public string ExecuteStoredFunctionVarchar2(string statement)
        {
            if (!this.IsConnected())
            {
                throw new Exception("No connection to Oracle Database");
            }
            cmd.CommandText = "BEGIN :1 := " + statement + "; END;";
            OracleParameter result = cmd.Parameters.Add("result", OracleDbType.Varchar2, 200, null, ParameterDirection.Output);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (OracleException oex)
            {
                Console.WriteLine(oex.Message);
            }
            return result.Value.ToString();
        }
    }
}
