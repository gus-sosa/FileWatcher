using System;
using System.Collections.Generic;
using System.Text;
using WatcherManager.Domain.Constants;

namespace WatcherManager.Infrastructure.Contracts
{
    public interface IDomainValidator
    {
        ValidationResult IsValid(AgentType agentType, object agentExtraParams);
    }
}
