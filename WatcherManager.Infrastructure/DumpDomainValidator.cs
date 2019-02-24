using WatcherManager.Domain.Constants;
using WatcherManager.Infrastructure.Contracts;

namespace WatcherManager.Infrastructure
{
    public class DumpDomainValidator : IDomainValidator
    {
        public ValidationResult IsValid(AgentType agentType, object agentExtraParams) => new ValidationResult() { IsValid = true };
    }
}
