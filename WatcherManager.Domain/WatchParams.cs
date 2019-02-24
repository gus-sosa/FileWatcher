using WatcherManager.Domain.Constants;

namespace WatcherManager.Domain
{
    public class WatchParams
    {
        public AgentType AgentType { get; set; }
        public object AgentExtraParams { get; set; }
        public bool RunForever => Schedule == null;
        public Schedule Schedule { get; set; }
    }
}
