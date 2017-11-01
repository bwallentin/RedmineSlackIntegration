using System.Collections.Generic;
using System.Linq;
using Redmine.Net.Api.Types;
using RedmineSlackIntegration.Domain.Redmine.Data;

namespace RedmineSlackIntegration.Domain.Redmine
{
    public interface IRedmineManager
    {
        IList<Issue> GetNewIssuesOrProdsattIssuesToBeSentToSlack();
        IList<Issue> GetIssuesInProgressToBeSentToSlack();
    }

    public class RedmineManager : IRedmineManager
    {
        private readonly IRedmineRepository _redmineRepo;
        private readonly IRedmineApiIntegration _redmineApiIntegration;
        
        public RedmineManager(IRedmineRepository redmineRepo, IRedmineApiIntegration redmineApiIntegration)
        {
            _redmineRepo = redmineRepo;
            _redmineApiIntegration = redmineApiIntegration;
        }
        
        public IList<Issue> GetNewIssuesOrProdsattIssuesToBeSentToSlack()
        {
            var issuesFromAdlis = _redmineApiIntegration.GetNewIssuesAndProdsattIssuesFromAdlis();
            var alreadyKnownIssues = _redmineRepo.GetAlreadyKnownIssuesFromDb();
            
            var issuesFromAdlisOnlyStrings = RedmineHelper.ConvertListFromAdlisToListWithOnlyStrings(issuesFromAdlis);
            var returnListOnlyStrings = issuesFromAdlisOnlyStrings.Except(alreadyKnownIssues).ToList();
            
            _redmineRepo.UpdateDbWithKnownIssues(issuesFromAdlis);

            return !returnListOnlyStrings.Any() ?
                new List<Issue>() :
                RedmineHelper.CreateReturnList(issuesFromAdlis, returnListOnlyStrings);
        }

        public IList<Issue> GetIssuesInProgressToBeSentToSlack()
        {
            var completeListFromAdlis = _redmineApiIntegration.GetIssuesInProgress();
            
            RedmineHelper.RemoveDailyBusinessIssues(completeListFromAdlis);
            RedmineHelper.RemoveBlockedIssues(completeListFromAdlis);
            RedmineHelper.RemoveExcludedUsers(completeListFromAdlis);

            return completeListFromAdlis;
        }
    }
}
