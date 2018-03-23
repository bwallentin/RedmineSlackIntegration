using System;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using RedmineSlackIntegration.Domain.Configuration;
using RedmineSlackIntegration.Domain.Redmine;
using RedmineSlackIntegration.Domain.Slack;
using RedmineSlackIntegration.Jobs;

namespace RedmineSlackIntegration
{
    public interface IRedmineSlackIntegrationService
    {
        void WhenStarted();
        void WhenStopped();
    }

    public class RedmineSlackIntegrationService : IRedmineSlackIntegrationService
    {
        private static IScheduler _scheduler;
        private readonly IRedmineManager _redmineManager;
        private readonly ISlackClient _slackClient;

        public RedmineSlackIntegrationService(IRedmineManager redmineManager, ISlackClient slackClient)
        {
            _redmineManager = redmineManager;
            _slackClient = slackClient;
        }

        public void WhenStarted()
        {
            ISchedulerFactory schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler();
            _scheduler.Start();
            Console.WriteLine("Starting Scheduler");

            AddGetNewOrProdsattIssuesJob();
            //AddCheckWipLimitJob();
            //AddPostStormIntegrationStatusJob();
        }

        public void WhenStopped()
        {
        }

        public void AddGetNewOrProdsattIssuesJob()
        {
            var cronScheduele = ConfigurationProvider.GetNewOrProdsattIssuesCronSchedule;
            
            IGetNewOrProdsattIssuesJob my = new GetNewOrProdsattIssuesJob(_slackClient, _redmineManager);
            var jobDetail = new JobDetailImpl("Job1", "Group1", my.GetType());
            var trigger = new CronTriggerImpl("Trigger1", "Group1", cronScheduele);
            _scheduler.ScheduleJob(jobDetail, trigger);
        }

        public void AddCheckWipLimitJob()
        {
            var cronScheduele = ConfigurationProvider.CheckWipLimitJobCronSchedule;

            ICheckWipLimitJob my = new CheckWipLimitJob(_slackClient, _redmineManager);
            var jobDetail = new JobDetailImpl("Job2", "Group2", my.GetType());
            var trigger = new CronTriggerImpl("Trigger2", "Group2", cronScheduele);
            _scheduler.ScheduleJob(jobDetail, trigger);
        }

        public void AddPostStormIntegrationStatusJob()
        {
            var cronScheduele = ConfigurationProvider.PostStormIntegrationStatusCronSchedule;

            IPostStormIntegrationStatusJob my = new PostStormIntegrationStatusJob(_slackClient);
            var jobDetail = new JobDetailImpl("Job3", "Group3", my.GetType());
            var trigger = new CronTriggerImpl("Trigger3", "Group3", cronScheduele);
            _scheduler.ScheduleJob(jobDetail, trigger);
        }
    }
}
