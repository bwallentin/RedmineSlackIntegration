using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace RedmineSlackIntegration.DataUtils
{
    public class SqlManager : IDisposable
    {
        private readonly string _connectionString;
        private readonly string _scriptsDirectory;
        private const string ScriptsDirectory = "Sql";
        private readonly int _connectionTimeout;

        public SqlManager(string connectionString, string scriptsDirectory = null, int connectionTimeout = 60)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            _connectionString = connectionString;
            _connectionTimeout = connectionTimeout;
            _scriptsDirectory = string.IsNullOrWhiteSpace(scriptsDirectory) ? ScriptsDirectory : scriptsDirectory;
        }

        public string GetSqlScriptContentsFromEmbeddedResource(string scriptFile, Assembly callingAssembly = null, string callerNamespace = null, string resourcePath = null)
        {
            if (callingAssembly == null)
            {
                callingAssembly = Assembly.GetCallingAssembly();
            }

            if (callerNamespace == null)
            {
                callerNamespace = ReflectionProvider.GetCallerNamespace(new StackFrame(1));
            }

            if (scriptFile.ToLowerInvariant().EndsWith(".sql"))
            {
                throw new ArgumentException("scriptFile name should not include sql extension", nameof(scriptFile));
            }

            string query = null;

            if (!string.IsNullOrWhiteSpace(resourcePath))
            {
                query = GetEmbeddedResource(callingAssembly, resourcePath + "." + scriptFile);
            }

            if (query == null)
            {
                query =
                    GetEmbeddedResource(callingAssembly, GetResourceName(callerNamespace, scriptFile))
                    ??
                    GetEmbeddedResource(callingAssembly, GetResourceName(callingAssembly.GetName().Name, scriptFile));
            }

            if (query == null)
            {
                var currentAssembly = Assembly.GetExecutingAssembly();
                query = GetEmbeddedResource(currentAssembly, GetResourceName(currentAssembly.GetName().Name, scriptFile));
                if (query == null) throw new Exception($"Resource '{scriptFile}' not found.");
            }

            return query;
        }

        private string GetResourceName(string name, string scriptFile)
        {
            if (string.IsNullOrWhiteSpace(name)) return null;
            if (name.ToLowerInvariant().Trim().StartsWith("mscorlib.")) name = name.Substring(9);
            return $"{name}.{_scriptsDirectory}.{scriptFile}.sql";
        }

        private SqlCommand BuildSqlTextCommand(string sqlScriptName, SqlConnection conn, Assembly callingAssembly, string callerNamespace, string resourcePath = null)
        {
            return new SqlCommand
            {
                Connection = conn,
                CommandType = CommandType.Text,
                CommandTimeout = _connectionTimeout,
                CommandText = GetSqlScriptContentsFromEmbeddedResource(sqlScriptName, callingAssembly, callerNamespace, resourcePath)
            };
        }

        private static void AddParameters(List<SqlParameter> parameters, SqlCommand cmd)
        {
            if (parameters == null) return;
            foreach (var parameter in parameters)
            {
                cmd.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
            }
        }
        
        public DataTable ExecuteScriptFileGetDataTable(string sqlScriptName, List<SqlParameter> parameters = null, string scriptResourcePath = null, Assembly callingAssembly = null)
        {
            if (callingAssembly == null) callingAssembly = Assembly.GetCallingAssembly();
            var callerNamespace = ReflectionProvider.GetCallerNamespace(new StackFrame(1));

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = BuildSqlTextCommand(sqlScriptName, conn, callingAssembly, callerNamespace, scriptResourcePath))
                {
                    AddParameters(parameters, cmd);
                    conn.Open();
                    var dt = new DataTable();
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    return dt;
                }
            }
        }
        
        public int ExecuteScriptFileNonQuery(string sqlScriptName, List<SqlParameter> parameters = null, bool allowDangerousDirectParamInsert = false, string scriptResourcePath = null, Assembly callingAssembly = null)
        {
            if (callingAssembly == null) callingAssembly = Assembly.GetCallingAssembly();
            var callerNamespace = ReflectionProvider.GetCallerNamespace(new StackFrame(1));

            using (var conn = new SqlConnection(_connectionString))
            {
                using (var cmd = BuildSqlTextCommand(sqlScriptName, conn, callingAssembly, callerNamespace, scriptResourcePath))
                {
                    AddParameters(parameters, cmd);
                    conn.Open();

                    var rows = cmd.ExecuteNonQuery();

                    conn.Close();
                    return rows;
                }
            }
        }

        public static string GetEmbeddedResource(Assembly assembly, string resourceName, bool throwException = false)
        {
            string query;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        query = reader.ReadToEnd();
                    }
                }
                else
                {
                    if (throwException)
                    {
                        throw new Exception($"Could not find embedded resource {resourceName}");
                    }
                    return null;
                }
            }
            return query;
        }

        public void Dispose()
        {
        }
    }
}
