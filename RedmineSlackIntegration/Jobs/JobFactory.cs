using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Quartz;
using Quartz.Spi;
using SimpleInjector;

namespace RedmineSlackIntegration.Jobs
{
    public class SimpleInjectorJobFactory : IJobFactory
    {
        private readonly Dictionary<Type, InstanceProducer> _jobProducers;

        public SimpleInjectorJobFactory(Container container, Assembly[] assemblies)
        {
            var types = container.GetTypesToRegister(typeof(IJob), assemblies);

            var lifestyle = Lifestyle.Transient;

            // By creating producers here by the IJob service type, jobs can be decorated.
            this._jobProducers = (
                from type in types
                let producer = lifestyle.CreateProducer(typeof(IJob), type, container)
                select new { type, producer })
                .ToDictionary(t => t.type, t => t.producer);
        }

        public IJob NewJob(TriggerFiredBundle bundle)
        {
            return (IJob)this._jobProducers[bundle.JobDetail.JobType].GetInstance();
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            return (IJob)this._jobProducers[bundle.JobDetail.JobType].GetInstance();
        }

        public void ReturnJob(IJob job)
        {
        }
    }
}
