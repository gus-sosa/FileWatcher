using System.IO;
using FileWatcher.Abstracts.Contracts;
using FileWatcher.Abstracts.Domain;
using Serilog;

namespace FileWatcher.Core {
  public class FileWatcher : IFileWatcher {
    private readonly FolderWatchMetadata _folder;
    private readonly FileSystemWatcher _watcher;
    private readonly IDispatcher dispatcher;
    private readonly ILogger logger;

    public FileWatcher(FolderWatchMetadata folder, IDispatcher dispatcher, ILogger logger) {
      this._folder = folder;
      this._watcher = initializeWatcher();
      this.dispatcher = dispatcher;
      this.logger = logger;
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

    private void newFileDeleted(object sender, FileSystemEventArgs e) => dispatcher.DispatchMessage(createDeleteFileMessageInfo(e));

    private object createDeleteFileMessageInfo(FileSystemEventArgs e) =>
      new DeleteFileMessage() {
        FilePath = e.FullPath,
        FileName = e.Name
      };

    private void newFileCreated(object sender, FileSystemEventArgs e) => dispatcher.DispatchMessage(createNewFileMessageInfo(e));

    private object createNewFileMessageInfo(FileSystemEventArgs e) =>
      new NewFileMessage() {
        FilePath = e.FullPath,
        FileName = e.Name
      };

    public void Dispose() {
      Stop();
      _watcher.Dispose();
    }

    public void Start() => _watcher.EnableRaisingEvents = true;

    public void Stop() => _watcher.EnableRaisingEvents = false;
  }
}
