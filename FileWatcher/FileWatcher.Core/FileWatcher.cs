using System;
using FileWatcher.Abstracts.Contracts;
using FileWatcher.Abstracts.Domain;

namespace FileWatcher.Core {
  public class FileWatcher : IFileWatcher {
    private FolderWatchMetadata _folder;

    public FileWatcher(FolderWatchMetadata folder) {
      this._folder = folder;
    }

    public void Dispose() => Stop();

    public void Start() {
      throw new NotImplementedException();
    }

    public void Stop() {
      throw new NotImplementedException();
    }
  }
}
