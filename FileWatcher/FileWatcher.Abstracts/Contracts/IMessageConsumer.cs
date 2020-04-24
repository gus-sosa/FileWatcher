using System;
using System.Threading.Tasks;

namespace FileWatcher.Abstracts.Contracts {
  public interface IMesssageConsumer<in T> : IDisposable {
    Task ConsumeMessage(T message);
  }
}
