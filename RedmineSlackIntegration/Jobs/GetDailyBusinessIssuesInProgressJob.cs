using System.Linq;
using Quartz;
using RedmineSlackIntegration.Redmine;
using RedmineSlackIntegration.Slack;

namespace RedmineSlackIntegration.Jobs
{
    internal interface IGetDailyBusinessIssuesInProgressJob : IJob
    {
    }

    internal class GetDailyBusinessIssuesInProgressJob : IGetDailyBusinessIssuesInProgressJob
    {
        private readonly ISlackClient _slackClient;
        private readonly IRedmineIntegration _redmineIntegration;

        public GetDailyBusinessIssuesInProgressJob(ISlackClient slackClient, IRedmineIntegration redmineIntegration)
        {
            _slackClient = slackClient;
            _redmineIntegration = redmineIntegration;
        }

        public void Execute(IJobExecutionContext context)
        {
            var issues = _redmineIntegration.GetDailyBusinessIssuesToBeSentToSlack();
            if (issues.Any() && issues.Count > 2)
            {
                _slackClient.PostDailyBusinessWarningToSlack();
            }
        }
    }
}
