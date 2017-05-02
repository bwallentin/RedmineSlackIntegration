using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Redmine.Net.Api.Types;
using RedmineSlackIntegration.Domain.Configuration.Data;

namespace RedmineSlackIntegration.Domain.Redmine
{
    public interface IRedmineApiIntegration
    {
        List<Issue> GetNewIssuesAndProdsattIssuesFromAdlis();
        List<Issue> GetIssuesInProgress();
        //IList<Project> GetProjects();
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
        //private const int DailyBusinessProjectId = (int)RedmineProjects.DailyBusiness;
        private const int InkopProduktProjectId = (int)RedmineProjects.InkopProdukt;

        private readonly global::Redmine.Net.Api.RedmineManager _redmineApiManager;

        public RedmineApiIntegration(IConfigurationRepo configurationRepo)
        {
            _redmineApiManager = new global::Redmine.Net.Api.RedmineManager(@"http://adlis/", configurationRepo.AdlisApiKey);
        }
        
        public List<Issue> GetNewIssuesAndProdsattIssuesFromAdlis()
        {
            var issues = new List<Issue>();
            issues.AddRange(GetIssues(KlarForDevStatusId, InkopProduktProjectId).Select(issue => issue).Where(x => x.AssignedTo == null).ToList());
            issues.AddRange(GetIssues(ProdsattStatusId, InkopProduktProjectId));

            // We probably don't need to extract the DailyBusinessProject issues because that's a subproject to InkopProduktProject
            //issues.AddRange(GetIssues(KlarForDevStatusId, DailyBusinessProjectId).Select(issue => issue).Where(x => x.AssignedTo == null).ToList());
            //issues.AddRange(GetIssues(ProdsattStatusId, DailyBusinessProjectId));
            return issues;
        }
        
        public List<Issue> GetIssuesInProgress()
        {
            var issues = new List<Issue>();
            issues.AddRange(GetIssues(UtvecklingStatusId, InkopProduktProjectId));
            issues.AddRange(GetIssues(DemoStatusId, InkopProduktProjectId));
            issues.AddRange(GetIssues(VerifieringStatusId, InkopProduktProjectId));
            issues.AddRange(GetIssues(AcctestStatusId, InkopProduktProjectId));
            return issues;
        }

        // Get all projects from Adlis
        //public IList<Project> GetProjects()
        //{
        //    var parameters = new NameValueCollection { { "limit", "100" } };
        //    var projs = _redmineApiManager.GetObjectList<Project>(parameters);
        //    return projs;
        //}

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
