using System;
using System.Threading.Tasks;

namespace FileWatcher.Abstracts.Contracts {
  public interface IMesssageConsumer<T> : IDisposable {
    Task ConsumeMessage(T message);
  }
}
