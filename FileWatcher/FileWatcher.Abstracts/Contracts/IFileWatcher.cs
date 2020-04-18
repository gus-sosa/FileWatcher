using System;

namespace FileWatcher.Abstracts.Contracts {
  public interface IFileWatcher : IDisposable {
    void Start();
    void Stop();
  }
}
