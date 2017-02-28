using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using RedmineSlackIntegration.DataUtils;

namespace RedmineSlackIntegration.Domain.Configuration.Data
{
    public interface IConfigurationRepo
    {
        string AdlisApiKey { get; }
        string SlackHook { get; }
    }

    public class ConfigurationRepo : DataRepository, IConfigurationRepo
    {
        public string AdlisApiKey => GetConfiguration("AdlisApiKey");
        public string SlackHook => GetConfiguration("SlackHook");

        public ConfigurationRepo(string connectionString) : base(connectionString)
        {
        }

        public string GetConfiguration(string value)
        {
            string config;

            using (var sqlManager = GetSqlManagerInstance())
            {
                var p = new List<SqlParameter>
                {
                    new SqlParameter("Id", SqlDbType.VarChar) {Value = value}
                };

                var dt = sqlManager.ExecuteScriptFileGetDataTable("GetConfiguration", p);
                var list = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("Value")).ToList();
                config = list.FirstOrDefault();
            }

            return config;
        }
    }
}
