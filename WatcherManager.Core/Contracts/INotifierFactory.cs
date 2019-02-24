using System;
using System.Collections.Generic;
using System.Text;

namespace WatcherManager.Core.Contracts
{
    public interface INotifierFactory
    {
        INotifier GetNotifier();
    }
}
