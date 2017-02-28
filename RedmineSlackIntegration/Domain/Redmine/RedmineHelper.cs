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
    }
}
