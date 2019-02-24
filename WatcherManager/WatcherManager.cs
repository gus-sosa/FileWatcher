using System;
using WatcherManager.Core;
using WatcherManager.Core.Contracts;
using WatcherManager.Domain;
using WatcherManager.Infrastructure;
using WatcherManager.Infrastructure.Contracts;

namespace WatcherManager
{
    public class WatcherManager : IWatcherManager
    {
        private readonly IAgentFactory agentFactory;
        private readonly INotifierFactory notifierFactory;

        public WatcherManager(IAgentFactory agentFactory, INotifierFactory notifierFactory)
        {
            this.agentFactory = agentFactory;
            this.notifierFactory = notifierFactory;
        }

        public IDomainValidator DomainValidator { get; set; } = new DumpDomainValidator();

        public void Watch(WatchParams watchParams)
        {
            if (watchParams.RunForever)
                WatchForever(GetAgent(watchParams));
            else
            {
                IAgent agentConfigurator = GetAgent(watchParams);
                INotifier notifier = notifierFactory.GetNotifier();//TODO: Define all the things related to the notifier correctly
                ScheduleAgentWork(watchParams.Schedule, agentConfigurator, notifier);
            }
        }

        private IAgent GetAgent(WatchParams watchParams) => GetAgentConfigurator(watchParams)();

        private Func<IAgent> GetAgentConfigurator(WatchParams watchParams)
        {
            ValidationResult agentValidation = DomainValidator.IsValid(watchParams.AgentType, watchParams.AgentExtraParams);
            if (!agentValidation.IsValid)
            {
                //TODO: Specify exception properly
                throw new System.Exception();
            }

            AgentConfigurator agentConfigurator = agentFactory.GetAgentConfigurator(watchParams.AgentType, watchParams.AgentExtraParams);
            if (!agentConfigurator.IsValidAgent)
            {
                //TODO: Specify exception properly
                throw new Exception();
            }

            return agentConfigurator.GetAgent;
        }

        private void WatchForever(IAgent agent)
        {
            throw new NotImplementedException();
        }

        private void ScheduleAgentWork(Schedule schedule, IAgent agent, INotifier notifier)
        {
            throw new NotImplementedException();
        }
    }
}
