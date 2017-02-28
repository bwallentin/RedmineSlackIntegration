using System;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using RedmineSlackIntegration.Domain.Redmine;
using RedmineSlackIntegration.Domain.Redmine.Data;
using RedmineSlackIntegration.Domain.Slack;
using RedmineSlackIntegration.Jobs;
using SimpleInjector;
using Topshelf;

namespace RedmineSlackIntegration.Bootstrap
{
    internal static class ConfigureService
    {
        internal static void Configure()
        {
            var container = new Container();
            Register(container);
            SetupService(container);
        }

        private static void Register(Container container)
        {
            var schedulerFactory = new StdSchedulerFactory();
            container.RegisterSingleton<ISchedulerFactory>(schedulerFactory);

            container.Register<ISlackClient, SlackClient>();
            container.Register<IRedmineManager, RedmineManager>();
            container.Register<IRedmineApiIntegration, RedmineApiIntegration>();
            container.Register<IRedmineRepository>(() => new RedmineRepository(ConfigurationProvider.ConnectionString));
            container.Register<IRedmineSlackIntegrationService, RedmineSlackIntegrationService>();

            container.RegisterSingleton<IScheduler>(() =>
            {
                var scheduler = schedulerFactory.GetScheduler();
                scheduler.JobFactory = container.GetInstance<IJobFactory>();
                return scheduler;
            });

            var applicationAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            container.RegisterSingleton<IJobFactory>(new SimpleInjectorJobFactory(container, applicationAssemblies));

            container.Verify();
        }

        private static void SetupService(Container container)
        {
            HostFactory.Run(configure =>
            {
                configure.Service<IRedmineSlackIntegrationService>(service =>
                {
                    service.ConstructUsing(s => container.GetInstance<IRedmineSlackIntegrationService>());
                    service.WhenStarted(s => s.WhenStarted());
                    service.WhenStopped(s => s.WhenStopped());
                });

#if DEBUG
                var serviceName = "RedmineSlackIntegrationDebug";
#else
                var serviceName = "RedmineSlackIntegration";
#endif

                //Setup Account that window service use to run.  
                configure.RunAsLocalSystem();
                configure.SetServiceName(serviceName);
                configure.SetDisplayName(serviceName);
                configure.SetDescription("Integration between Adlis and Slack");
            });
        }
    }
}
