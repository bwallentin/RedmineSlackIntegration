using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;

namespace RedmineSlackIntegration.Redmine
{
    public interface IRedmineIntegration
    {
        IList<Issue> GetReadyForDevOrProdsattIssuesToBeSentToSlack();
        IList<Issue> GetDailyBusinessIssuesToBeSentToSlack();
    }

    public class RedmineIntegration : IRedmineIntegration
    {
        // Status Id
        private const int KlarForDevStatusId = (int)RedmineStatus.KlarForDev;
        private const int UtvecklingStatusId = (int)RedmineStatus.Utveckling;
        private const int AcctestStatusId = (int)RedmineStatus.Acctest;
        private const int DemoStatusId = (int)RedmineStatus.Demo;
        private const int VerifieringStatusId = (int)RedmineStatus.Verifiering;
        private const int ProdsattStatusId = (int)RedmineStatus.Prodsatt;
        
        // Project Id
        private const int DailyBusinessProjectId = (int)RedmineProjects.DailyBusiness;
        //private const int AisProjectId = (int)RedmineProjects.AIS;
        private const int InkopProduktProjectId = (int)RedmineProjects.InkopProdukt;

        private readonly RedmineManager _manager;

        public RedmineIntegration()
        {
            _manager = new RedmineManager(ConfigurationProvider.AdlisHost, ConfigurationProvider.AdlisApiKey);
        }

        public IList<Issue> GetReadyForDevOrProdsattIssuesToBeSentToSlack()
        {
            var completeListFromAdlis = new List<Issue>();
            GetReadyForDevAndProdsattIssues(completeListFromAdlis);

            var issuesToBeSentToSlack = RedmineHelper.GetIssuesToBeSentToSlack(completeListFromAdlis);
            return issuesToBeSentToSlack;
        }

        public IList<Issue> GetDailyBusinessIssuesToBeSentToSlack()
        {
            var completeListFromAdlis = GetDailyBusinessIssuesInProgress();
            return completeListFromAdlis;
        }

        private void GetReadyForDevAndProdsattIssues(List<Issue> completeListFromAdlis)
        {
            var issuesReadyForDev = GetIssues(KlarForDevStatusId, InkopProduktProjectId);
            completeListFromAdlis.AddRange(issuesReadyForDev.Select(issue => issue).Where(x => x.AssignedTo == null).ToList());

            var issuesProdsatt = GetIssues(ProdsattStatusId, InkopProduktProjectId);
            completeListFromAdlis.AddRange(issuesProdsatt.Select(issue => issue).ToList());
        }
        
        private IList<Issue> GetDailyBusinessIssuesInProgress()
        {
            // TODO: Find a way not have to make three separate calls
            var issues = new List<Issue>();
            issues.AddRange(GetIssues(UtvecklingStatusId, DailyBusinessProjectId));
            issues.AddRange(GetIssues(DemoStatusId, DailyBusinessProjectId));
            issues.AddRange(GetIssues(VerifieringStatusId, DailyBusinessProjectId));
            issues.AddRange(GetIssues(AcctestStatusId, DailyBusinessProjectId));

            return issues;
        }

        private IList<Issue> GetIssues(int statusId, int projectId)
        {
            var parameters = new NameValueCollection
            {
                {"status_id", statusId.ToString()},
                {"project_id", projectId.ToString()}
            };

            var issues = _manager.GetObjectList<Issue>(parameters);

            return issues;
        }
    }
}
