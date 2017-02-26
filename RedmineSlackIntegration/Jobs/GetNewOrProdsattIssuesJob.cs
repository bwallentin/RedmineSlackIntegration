using System.Linq;
using Quartz;
using RedmineSlackIntegration.Redmine;
using RedmineSlackIntegration.Slack;

namespace RedmineSlackIntegration.Jobs
{
    internal interface IGetNewOrProdsattIssuesJob : IJob
    {
    }

    internal class GetNewOrProdsattIssuesJob : IGetNewOrProdsattIssuesJob
    {
        private readonly ISlackClient _slackClient;
        private readonly IRedmineIntegration _redmineIntegration;

        public GetNewOrProdsattIssuesJob(ISlackClient slackClient, IRedmineIntegration redmineIntegration)
        {
            _slackClient = slackClient;
            _redmineIntegration = redmineIntegration;
        }

        public void Execute(IJobExecutionContext context)
        {
            var issues = _redmineIntegration.GetReadyForDevOrProdsattIssuesToBeSentToSlack();
            if (issues.Any())
            {
                _slackClient.PostIssuesToSlack(issues);
            }
        }
    }
}
