using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using Serilog;

namespace Oracle4Net
{
    public class OracleDatabase : IOracleDatabase
    {

        private OracleConnection con;
        private OracleConnectionStringBuilder str;
        private OracleCommand cmd;
        private OracleDataReader rdr;
        private ILogger logger;

        public OracleDatabase()
        {
            this.con = new OracleConnection();
            this.str = new OracleConnectionStringBuilder();
            this.cmd = new OracleCommand();
            this.logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            logger.Information("Oracle Database created");
        }

        public void Connect(string ConnectionString)
        {
            str.ConnectionString = ConnectionString;
            con.ConnectionString = str.ToString();
            try
            {
                con.Open();
                cmd.Connection = con;
                logger.Information($"Connected to Oracle Database {con.ServerVersion}");
                logger.Information($"Connected as {str.UserID}@{con.DatabaseName}");
            }
            catch (OracleException oex)
            {
                throw new OracleDatabaseException(oex.Message, oex.Number);
            }
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
                logger.Information($"Connected to Oracle Database {con.ServerVersion}");
                logger.Information($"Connected as {str.UserID}@{con.DatabaseName}");
            }
            catch (OracleException oex)
            {
                throw new OracleDatabaseException(oex.Message, oex.Number);
            }
        }

        public bool IsConnected() => con.State == ConnectionState.Open;

        public void Disconnect()
        {
            string serverVersion = con.ServerVersion;
            con.Close();
            con.Dispose();
            logger.Information($"Disconnected from Oracle Database {serverVersion}");
        }

        private void ClearCommand()
        {
            cmd.CommandText = "BEGIN NULL; END;";
            cmd.Parameters.Clear();
        }

        public int ExecuteSQLCount(string table, string whereClause)
        {
            if (!this.IsConnected())
            {
                throw new OracleDatabaseException("No connection to Oracle Database", -20000);
            }
            int count = 0;
            cmd.CommandText = "SELECT COUNT(*) FROM " + table + " WHERE " + whereClause;
            try
            {
                count = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (OracleException oex)
            {
                throw new OracleDatabaseException(oex.Message, oex.Number);
            }
            ClearCommand();
            return count;
        }

        public void ExecuteSQL(string sql)
        {
            if (!this.IsConnected())
            {
                throw new Exception("No connection to Oracle Database");
            }
            int count = 0;
            cmd.CommandText = "BEGIN OPEN :1 FOR :2; END;";
            OracleParameter refcursor = cmd.Parameters.Add("refcursor", OracleDbType.RefCursor, ParameterDirection.Output);
            OracleParameter query = cmd.Parameters.Add("query", OracleDbType.Varchar2, 4000, sql, ParameterDirection.Input);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (OracleException oex)
            {
                logger.Error(oex.Message);
            }
            rdr = ((OracleRefCursor)refcursor.Value).GetDataReader();
            if (!rdr.HasRows)
                logger.Information("No rows selected");
            else
            {
                DataTable table = rdr.GetSchemaTable();
                DataSet set = new DataSet();
                set.EnforceConstraints = false;
                set.Tables.Add(table);
                set.Load(rdr, LoadOption.OverwriteChanges, table);
                /*for (int i = 0; i < rdr.FieldCount; i++)
                {
                    Console.WriteLine(rdr.GetName(i));
                }

                while (rdr.Read())
                {
                    count++;
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        if (!rdr.IsDBNull(i))
                        {
                            Console.WriteLine(rdr.GetValue(i));
                        }
                    }
                }*/
                foreach (DataColumn col in table.Columns)
                {
                    Console.Write(col.ColumnName);
                }
                Console.WriteLine();

                Console.WriteLine("--------------------------------------");

                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn col in table.Columns)
                    {
                        Console.Write(row[col] + " ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
                //Console.WriteLine(set.GetXml());

                logger.Information($"{count} rows selected");
            }
            ClearCommand();
        }

        public void ExecuteSQLSelect(string sql)
        {
            if (!this.IsConnected())
            {
                throw new OracleDatabaseException("No connection to Oracle Database", -20000);
            }
            int count = 0;
            cmd.CommandText = sql;

            try
            {
                rdr = cmd.ExecuteReader();
            }
            catch (OracleException oex)
            {
                throw new OracleDatabaseException(oex.Message, oex.Number);
            }

            if (!rdr.HasRows)
                logger.Information("No rows selected");
            else
            {

                for (int i = 0; i < rdr.FieldCount; i++)
                {
                    Console.Write(rdr.GetName(i) + " ");
                }
                Console.WriteLine();

                while (rdr.Read())
                {
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        if (!rdr.IsDBNull(i))
                        {
                            if (rdr.GetFieldType(i).Equals(typeof(System.Byte[])))
                            {
                                Console.Write("<BLOB> ");
                            }
                            else
                            {
                                Console.Write(rdr.GetValue(i) + " ");
                            }
                        }
                    }
                    count++;
                    Console.WriteLine();
                }

                logger.Information($"{count} row(s) selected");
            }
            ClearCommand();
        }

        public T ExecuteStoredFunction<T>(string statement) // where T : System.Decimal
        {
            if (!this.IsConnected())
            {
                throw new OracleDatabaseException("No connection to Oracle Database", -20000);
            }
            cmd.CommandText = "BEGIN :1 := " + statement + "; END;";
            OracleParameter result = new OracleParameter();
            result.Direction = ParameterDirection.Output;

            // * OracleDbTypeEx specifies the Oracle data type to bind the parameter as, but returns a .NET type as output
            if (typeof(T).Equals(typeof(System.String)))
            {
                result.OracleDbType = OracleDbType.Clob;
                result.OracleDbTypeEx = OracleDbType.Clob;
                // ? Size to be defined
                result.Size = 32000;
            }
            else if (typeof(T).Equals(typeof(System.Decimal)))
            {
                result.OracleDbType = OracleDbType.Decimal;
                result.OracleDbTypeEx = OracleDbType.Decimal;
                result.Size = 0;
            }
            else
            {
                throw new OracleDatabaseException("Return Type not allowed", -20000);
            }

            cmd.Parameters.Add(result);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (OracleException oex)
            {
                throw new OracleDatabaseException(oex.Message, oex.Number);
            }
            ClearCommand();
            return (T)result.Value;
        }

        public void ExecuteStoredProcedure(string statement)
        {
            if (!this.IsConnected())
            {
                throw new OracleDatabaseException("No connection to Oracle Database", -20000);
            }
            cmd.CommandText = "BEGIN " + statement + "; END;";
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (OracleException oex)
            {
                throw new OracleDatabaseException(oex.Message, oex.Number);
            }
            ClearCommand();
        }

        public void ExecutePLSQLBlock(string block)
        {
            if (!this.IsConnected())
            {
                throw new OracleDatabaseException("No connection to Oracle Database", -20000);
            }
            cmd.CommandText = block;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (OracleException oex)
            {
                throw new OracleDatabaseException(oex.Message, oex.Number);
            }
            ClearCommand();
        }

    }
}