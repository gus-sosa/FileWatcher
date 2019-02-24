using System;
using System.Collections.Generic;
using System.Text;
using WatcherManager.Domain.Constants;

namespace WatcherManager.Core.Contracts
{
    public interface IAgentFactory
    {
        AgentConfigurator GetAgentConfigurator(AgentType agentType, object agentExtraParams);
    }
}
