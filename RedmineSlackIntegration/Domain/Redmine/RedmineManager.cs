using System.Collections.Generic;
using System.Linq;
using Redmine.Net.Api.Types;
using RedmineSlackIntegration.Domain.Redmine.Data;

namespace RedmineSlackIntegration.Domain.Redmine
{
    public interface IRedmineManager
    {
        IList<Issue> GetReadyForDevOrProdsattIssuesToBeSentToSlack();
        IList<Issue> GetDailyBusinessIssuesToBeSentToSlack();
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

        public IList<Issue> GetReadyForDevOrProdsattIssuesToBeSentToSlack()
        {
            var issuesFromAdlis = _redmineApiIntegration.GetReadyForDevAndProdsattIssuesFromAdlis();
            var alreadyKnownIssues = _redmineRepo.GetAlreadyKnownIssuesFromDb();
            
            var issuesFromAdlisOnlyStrings = RedmineHelper.ConvertListFromAdlisToListWithOnlyStrings(issuesFromAdlis);
            var returnListOnlyStrings = issuesFromAdlisOnlyStrings.Except(alreadyKnownIssues).ToList();
            
            _redmineRepo.UpdateDbWithKnownIssues(issuesFromAdlis);

            return !returnListOnlyStrings.Any() ?
                new List<Issue>() :
                RedmineHelper.CreateReturnList(issuesFromAdlis, returnListOnlyStrings);
        }

        public IList<Issue> GetDailyBusinessIssuesToBeSentToSlack()
        {
            var completeListFromAdlis = _redmineApiIntegration.GetDailyBusinessIssuesInProgress();
            completeListFromAdlis.RemoveAll(x => x.AssignedTo.Name.Contains("Lars Utterström"));
            return completeListFromAdlis;
        }
    }
}
