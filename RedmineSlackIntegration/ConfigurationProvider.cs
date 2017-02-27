using System;
using System.Configuration;

namespace RedmineSlackIntegration
{
    public static class ConfigurationProvider
    {
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

        public static string SlackHook => GetNonEmptyAppSetting("SlackHook");
        public static string AdlisHost => GetNonEmptyAppSetting("AdlisHost");
        public static string AdlisApiKey => GetNonEmptyAppSetting("AdlisApiKey");
        public static string AlreadyKnownIssuesJsonFile => GetNonEmptyAppSetting("AlreadyKnownIssuesJsonFile");
        public static string GetNewOrProdsattIssuesCronSchedule => GetNonEmptyAppSetting("GetNewOrProdsattIssuesCronSchedule");
        public static string GetDailyBusinessIssuesInProgressCronSchedule => GetNonEmptyAppSetting("GetDailyBusinessIssuesInProgressCronSchedule");
    }
}
