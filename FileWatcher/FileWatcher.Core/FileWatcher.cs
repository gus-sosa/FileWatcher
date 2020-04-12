using System;
using System.IO;
using FileWatcher.Abstracts.Contracts;
using FileWatcher.Abstracts.Domain;

namespace FileWatcher.Core {
  public class FileWatcher : IFileWatcher {
    private FolderWatchMetadata _folder;
    private FileSystemWatcher _watcher;

    public FileWatcher(FolderWatchMetadata folder) {
      this._folder = folder;
      this._watcher = initializeWatcher();
    }

    private FileSystemWatcher initializeWatcher() {
      var watcher = new FileSystemWatcher();
      watcher.Path = _folder.FolderPath;
      watcher.NotifyFilter = NotifyFilters.LastAccess
                                      | NotifyFilters.LastWrite
                                      | NotifyFilters.FileName
                                      | NotifyFilters.DirectoryName;
      watcher.Filter = "*.*";
      watcher.Created += newFileCreated;
      watcher.Deleted += newFileDeleted;
      return watcher;
    }

    private void newFileDeleted(object sender, FileSystemEventArgs e) {
      throw new NotImplementedException("new file deleted");
    }

    private void newFileCreated(object sender, FileSystemEventArgs e) {
      throw new NotImplementedException("new file created");
    }

    public void Dispose() {
      Stop();
      _watcher.Dispose();
    }

    public void Start() {
      _watcher.EnableRaisingEvents = true;
    }

    public void Stop() {
      _watcher.EnableRaisingEvents = false;
    }
  }
}
