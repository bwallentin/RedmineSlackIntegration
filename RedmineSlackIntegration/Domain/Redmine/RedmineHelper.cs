using System.Collections.Generic;
using System.Linq;
using Redmine.Net.Api.Types;

namespace RedmineSlackIntegration.Domain.Redmine
{
    public static class RedmineHelper
    {
        public static IEnumerable<string> ConvertListFromAdlisToListWithOnlyStrings(List<Issue> issuesFromAdlis)
        {
            return issuesFromAdlis.Select(issue => issue.Id.ToString()).ToList();
        }

        public static List<Issue> CreateReturnList(List<Issue> freshListFromAdlis, List<string> returnListOnlyStrings)
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

        public static List<Issue> RemoveExcludedUsers(List<Issue> issues)
        {
            issues.RemoveAll(issue => issue.AssignedTo.Name.Contains("Lars Utterström"));
            return issues;
        }

        public static List<Issue> RemoveDailyBusinessIssues(List<Issue> issues)
        {
            issues.RemoveAll(issue =>
            {
                var dailybusinessId = RedmineProjects.DailyBusiness;
                return issue.Project.Id == (int)dailybusinessId;
            });

            return issues;
        }

        public static List<Issue> RemoveBlockedIssues(List<Issue> issues)
        {
            foreach(var issue in issues.ToList())
            {
                var blockedIssue = issue.CustomFields.First(x => x.Name == "Blockerad av")?.Values.Select(y => y.Info != string.Empty);
                if(blockedIssue != null && blockedIssue.First())
                {
                    issues.Remove(issue);
                }
            }

            return issues;
        }
    }
}
