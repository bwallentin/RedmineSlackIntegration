using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Redmine.Net.Api.Types;

namespace RedmineSlackIntegration.Domain.Redmine
{
    public interface IRedmineApiIntegration
    {
        List<Issue> GetReadyForDevAndProdsattIssuesFromAdlis();
        List<Issue> GetDailyBusinessIssuesInProgress();
    }

    public class RedmineApiIntegration : IRedmineApiIntegration
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
        private const int InkopProduktProjectId = (int)RedmineProjects.InkopProdukt;

        private readonly global::Redmine.Net.Api.RedmineManager _redmineApiManager;

        public RedmineApiIntegration()
        {
            _redmineApiManager = new global::Redmine.Net.Api.RedmineManager(ConfigurationProvider.AdlisHost, ConfigurationProvider.AdlisApiKey);
        }

        public List<Issue> GetReadyForDevAndProdsattIssuesFromAdlis()
        {
            var returnList = new List<Issue>();

            var issuesReadyForDev = GetIssues(KlarForDevStatusId, InkopProduktProjectId);
            returnList.AddRange(issuesReadyForDev.Select(issue => issue).Where(x => x.AssignedTo == null).ToList());

            var issuesProdsatt = GetIssues(ProdsattStatusId, InkopProduktProjectId);
            returnList.AddRange(issuesProdsatt.Select(issue => issue).ToList());

            return returnList;
        }

        public List<Issue> GetDailyBusinessIssuesInProgress()
        {
            // TODO: Find a way not to have to make a bunch of separate calls
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

            var issues = _redmineApiManager.GetObjectList<Issue>(parameters);

            return issues;
        }
    }
}
