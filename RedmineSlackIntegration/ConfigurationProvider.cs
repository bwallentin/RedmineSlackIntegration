using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace RedmineSlackIntegration
{
    public static class ConfigurationProvider
    {
        private static string GetConnectionString(string connection)
        {
            return ConfigurationManager.ConnectionStrings[connection].ConnectionString;
        }

        private static string GetAppSetting(string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }

        private static string GetNonEmptyAppSetting(string setting)
        {
            var key = GetAppSetting(setting);
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new Exception($"Setting {key}not configured in app.config!");
            }
            return key;
        }

        private static readonly string ExecutableLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string SlackHook => GetNonEmptyAppSetting("SlackHook");
        public static string AdlisHost => GetNonEmptyAppSetting("AdlisHost");
        public static string AdlisApiKey => GetNonEmptyAppSetting("AdlisApiKey");
        public static string AlreadyKnownIssuesJsonFile => Path.Combine(ExecutableLocation, "Issues.json");
        public static string GetNewOrProdsattIssuesCronSchedule => GetNonEmptyAppSetting("GetNewOrProdsattIssuesCronSchedule");
        public static string GetDailyBusinessIssuesInProgressCronSchedule => GetNonEmptyAppSetting("GetDailyBusinessIssuesInProgressCronSchedule");
        public static string ConnectionString => GetConnectionString("bodb");
    }
}
