namespace Oracle4Net
{
    public interface IOracleDatabase
    {
        void Connect(string ConnectionString);

        void Connect(string UserID, string Password, string DataSource);

        bool IsConnected();

        void Disconnect();

        int ExecuteSQLCount(string table, string whereClause);

        void ExecuteSQL(string sql);

        void ExecuteSQLSelect(string sql);

        T ExecuteStoredFunction<T>(string statement);

        void ExecuteStoredProcedure(string statement);

        void ExecutePLSQLBlock(string block);
    }
}

