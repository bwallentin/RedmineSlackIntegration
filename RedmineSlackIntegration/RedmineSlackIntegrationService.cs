using System;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using RedmineSlackIntegration.Jobs;
using RedmineSlackIntegration.Redmine;
using RedmineSlackIntegration.Slack;

namespace RedmineSlackIntegration
{
    public interface IRedmineSlackIntegrationService
    {
        void WhenStarted();
        void WhenStopped();
        void AddGetNewOrProdsattIssuesJob();
    }

    public class RedmineSlackIntegrationService : IRedmineSlackIntegrationService
    {
        private static IScheduler _scheduler;
        private readonly IRedmineIntegration _redmineIntegration;
        private readonly ISlackClient _slackClient;

        public RedmineSlackIntegrationService(IRedmineIntegration redmineIntegration, ISlackClient slackClient)
        {
            _redmineIntegration = redmineIntegration;
            _slackClient = slackClient;
        }

        public void WhenStarted()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler();
            _scheduler.Start();
            Console.WriteLine("Starting Scheduler");
            
            AddGetNewOrProdsattIssuesJob();
            AddGetIssuesInProgressJob();
        }

        public void WhenStopped()
        {
        }

        public void AddGetNewOrProdsattIssuesJob()
        {
            var cronScheduele = ConfigurationProvider.GetNewOrProdsattIssuesCronSchedule;
            
            IGetNewOrProdsattIssuesJob myJob = new GetNewOrProdsattIssuesJob(_slackClient, _redmineIntegration);
            var jobDetail = new JobDetailImpl("Job1", "Group1", myJob.GetType());
            var trigger = new CronTriggerImpl("Trigger1", "Group1", cronScheduele);
            _scheduler.ScheduleJob(jobDetail, trigger);
        }

        private void AddGetIssuesInProgressJob()
        {
            var cronScheduele = ConfigurationProvider.GetDailyBusinessIssuesInProgressCronSchedule;

            IGetDailyBusinessIssuesInProgressJob myJob = new GetDailyBusinessIssuesInProgressJob(_slackClient, _redmineIntegration);
            var jobDetail = new JobDetailImpl("Job2", "Group2", myJob.GetType());
            var trigger = new CronTriggerImpl("Trigger2", "Group2", cronScheduele);
            _scheduler.ScheduleJob(jobDetail, trigger);
        }
    }
}
