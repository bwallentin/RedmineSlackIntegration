using System.Linq;
using Quartz;
using RedmineSlackIntegration.Domain.Redmine;
using RedmineSlackIntegration.Domain.Slack;

namespace RedmineSlackIntegration.Jobs
{
    internal interface IGetDailyBusinessIssuesInProgressJob : IJob
    {
    }

    internal class GetDailyBusinessIssuesInProgressJob : IGetDailyBusinessIssuesInProgressJob
    {
        private readonly ISlackClient _slackClient;
        private readonly IRedmineManager _redmineManager;

        public GetDailyBusinessIssuesInProgressJob(ISlackClient slackClient, IRedmineManager redmineManager)
        {
            _slackClient = slackClient;
            _redmineManager = redmineManager;
        }

        public void Execute(IJobExecutionContext context)
        {
            var issues = _redmineManager.GetDailyBusinessIssuesToBeSentToSlack();
            if (issues.Any() && issues.Count > 2)
            {
                _slackClient.PostDailyBusinessWarningToSlack();
            }
        }
    }
}
