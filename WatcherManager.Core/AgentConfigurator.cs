using System;
using WatcherManager.Core.Contracts;

namespace WatcherManager.Core
{
    public class AgentConfigurator
    {
        private readonly Func<IAgent> agentConfigurator;

        public AgentConfigurator(Func<IAgent> agentConfigurator) => this.agentConfigurator = agentConfigurator;

        public bool IsValidAgent { get; set; }

        public IAgent GetAgent() => agentConfigurator();
    }
}
