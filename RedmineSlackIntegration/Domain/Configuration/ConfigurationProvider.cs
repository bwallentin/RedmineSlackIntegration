using System;
using System.Configuration;

namespace RedmineSlackIntegration.Domain.Configuration
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
                throw new Exception($"Setting {key} not configured in app.config!");
            }
            return key;
        }

        public static string GetNewOrProdsattIssuesCronSchedule => GetNonEmptyAppSetting("GetNewOrProdsattIssuesCronSchedule");
        public static string GetIssuesInProgressCronSchedule => GetNonEmptyAppSetting("GetIssuesInProgressCronSchedule");
        public static string WipLimit => GetNonEmptyAppSetting("WipLimit");
        public static string ExcludedUsers => GetNonEmptyAppSetting("ExcludedUsers");
        public static string ConnectionString => GetConnectionString("bodb");
    }
}
