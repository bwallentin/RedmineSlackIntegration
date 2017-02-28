using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Redmine.Net.Api.Types;
using RedmineSlackIntegration.DataUtils;

namespace RedmineSlackIntegration.Domain.Redmine.Data
{
    public interface IRedmineRepository
    {
        IEnumerable<string> GetAlreadyKnownIssuesFromDb();
        void UpdateDbWithKnownIssues(IList<Issue> issues);
    }

    public class RedmineRepository : DataRepository, IRedmineRepository
    {
        public RedmineRepository(string connectionString) : base(connectionString)
        {
        }

        public IEnumerable<string> GetAlreadyKnownIssuesFromDb()
        {
            List<string> list;

            using (var sqlManager = GetSqlManagerInstance())
            {
                var dt = sqlManager.ExecuteScriptFileGetDataTable("GetIssues");
                list = dt.Rows.OfType<DataRow>().Select(dr => dr.Field<string>("Issue")).ToList();
            }

            return list;
        }

        public void UpdateDbWithKnownIssues(IList<Issue> issues)
        {
            using (var sqlManager = GetSqlManagerInstance())
            {
                sqlManager.ExecuteScriptFileNonQuery("DeleteAllIssues");

                foreach (var issue in issues)
                {
                    var p = new List<SqlParameter>
                    {
                        new SqlParameter("Issue", SqlDbType.VarChar) {Value = issue.Id.ToString()}
                    };

                    sqlManager.ExecuteScriptFileNonQuery("InsertNewIssues", p);
                }
            }
        }
    }
}
