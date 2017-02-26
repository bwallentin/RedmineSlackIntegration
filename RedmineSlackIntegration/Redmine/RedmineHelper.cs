using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Redmine.Net.Api.Types;

namespace RedmineSlackIntegration.Redmine
{
    public static class RedmineHelper
    {
        private static readonly string JsonFilePath = ConfigurationProvider.AlreadyKnownIssuesJsonFile;

        public static List<Issue> GetIssuesToBeSentToSlack(List<Issue> issuesFromAdlis)
        {
            var alreadyKnownIssues = ReadAlreadyKnownIssuesFromJson();
            var issuesFromAdlisOnlyStrings = ConvertListFromAdlisToListWithOnlyStrings(issuesFromAdlis);

            // We want to return only knew issues
            var returnListOnlyStrings = issuesFromAdlisOnlyStrings.Except(alreadyKnownIssues).ToList();

            // Reset the Json file with known issues
            UpdateJsonWithKnownIssues(issuesFromAdlis);

            return !returnListOnlyStrings.Any() ?
                new List<Issue>() :
                CreateReturnList(issuesFromAdlis, returnListOnlyStrings);
        }

        private static void UpdateJsonWithKnownIssues(IEnumerable<Issue> result)
        {
            File.WriteAllText(JsonFilePath, string.Empty);

            var json = JsonConvert.SerializeObject(result.Select(issue => issue.Id.ToString()).ToArray());
            File.WriteAllText(JsonFilePath, json);
        }

        private static IEnumerable<string> ReadAlreadyKnownIssuesFromJson()
        {
            List<string> alreadyKnownIssues;
            using (var r = new StreamReader(JsonFilePath))
            {
                var json = r.ReadToEnd();
                alreadyKnownIssues = JsonConvert.DeserializeObject<List<string>>(json);
            }
            return alreadyKnownIssues;
        }

        private static IEnumerable<string> ConvertListFromAdlisToListWithOnlyStrings(List<Issue> freshListFromAdlis)
        {
            return freshListFromAdlis.Select(issue => issue.Id.ToString()).ToList();
        }

        private static List<Issue> CreateReturnList(List<Issue> freshListFromAdlis, List<string> returnListOnlyStrings)
        {
            var returnList = new List<Issue>();
            foreach (var issue in freshListFromAdlis.ToList())
            {
                if (returnListOnlyStrings.Contains(issue.Id.ToString()))
                {
                    returnList.Add(issue);
                }
            }
            return returnList;
        }
    }
}
