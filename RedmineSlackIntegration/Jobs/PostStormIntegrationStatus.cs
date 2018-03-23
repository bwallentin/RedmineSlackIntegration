using Quartz;
using RedmineSlackIntegration.Domain.Slack;

namespace RedmineSlackIntegration.Jobs
{
    internal interface IPostStormIntegrationStatusJob : IJob
    {
    }

    public class PostStormIntegrationStatusJob : IPostStormIntegrationStatusJob
    {
        private readonly ISlackClient _slackClient;

        public PostStormIntegrationStatusJob(ISlackClient slackClient)
        {
            _slackClient = slackClient;
        }

        public void Execute(IJobExecutionContext context)
        {
            _slackClient.PostStormIntegrationStatus();
        }
    }
}
