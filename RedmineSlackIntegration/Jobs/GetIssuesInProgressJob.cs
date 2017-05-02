using System;
using System.Linq;
using Quartz;
using RedmineSlackIntegration.Domain.Configuration;
using RedmineSlackIntegration.Domain.Redmine;
using RedmineSlackIntegration.Domain.Slack;

namespace RedmineSlackIntegration.Jobs
{
    internal interface IGetIssuesInProgressJob : IJob
    {
    }

    internal class GetIssuesInProgressJob : IGetIssuesInProgressJob
    {
        private readonly ISlackClient _slackClient;
        private readonly IRedmineManager _redmineManager;

        public GetIssuesInProgressJob(ISlackClient slackClient, IRedmineManager redmineManager)
        {
            _slackClient = slackClient;
            _redmineManager = redmineManager;
        }

        public void Execute(IJobExecutionContext context)
        {
            var wipLimit = ConfigurationProvider.WipLimit;
            var issues = _redmineManager.GetIssuesInProgressToBeSentToSlack();

            if (issues.Any() && issues.Count > Convert.ToInt32(wipLimit))
            {
                _slackClient.PostWipLimitBroken();
            }
        }
    }
}
