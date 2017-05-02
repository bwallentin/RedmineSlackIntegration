using System.Linq;
using Quartz;
using RedmineSlackIntegration.Domain.Redmine;
using RedmineSlackIntegration.Domain.Slack;

namespace RedmineSlackIntegration.Jobs
{
    internal interface IGetNewOrProdsattIssuesJob : IJob
    {
    }

    internal class GetNewOrProdsattIssuesJob : IGetNewOrProdsattIssuesJob
    {
        private readonly ISlackClient _slackClient;
        private readonly IRedmineManager _redmineManager;

        public GetNewOrProdsattIssuesJob(ISlackClient slackClient, IRedmineManager redmineManager)
        {
            _slackClient = slackClient;
            _redmineManager = redmineManager;
        }

        public void Execute(IJobExecutionContext context)
        {
            var issues = _redmineManager.GetNewIssuesOrProdsattIssuesToBeSentToSlack();

            if (issues.Any())
            {
                _slackClient.PostIssuesToSlack(issues);
            }
        }
    }
}
