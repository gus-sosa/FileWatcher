using System;

namespace FileWatcher.Abstracts.Contracts {
  public interface IDispatcher : IDisposable {
    void DispatchMessage(object msg);

    IDispatcher RegisterConsumer<T>(IMesssageConsumer<T> consumer);
  }
}
