using WatcherManager.Domain;

namespace WatcherManager.Core.Contracts
{
    public interface IWatcherManager
    {
        void Watch(WatchParams watchParams);
    }
}
