namespace RedmineSlackIntegration.DataUtils
{
    public abstract class DataRepository
    {
        protected string ConnectionString { get; }

        protected DataRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public SqlManager GetSqlManagerInstance()
        {
            return GetSqlManagerInstance(ConnectionString);
        }

        public SqlManager GetSqlManagerInstance(string connectionString)
        {
            return new SqlManager(connectionString);
        }
    }
}
