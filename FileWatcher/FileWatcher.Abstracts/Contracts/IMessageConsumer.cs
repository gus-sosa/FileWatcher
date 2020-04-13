using System;

namespace FileWatcher.Abstracts.Contracts {
  public interface IMesssageConsumer<T> : IDisposable {
    void ConsumeMessage(T message);
  }
}
