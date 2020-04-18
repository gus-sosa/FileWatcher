using System.IO;
using FileWatcher.Abstracts.Contracts;
using FileWatcher.Abstracts.Domain;
using Newtonsoft.Json;
using Serilog;

namespace FileWatcher.Core {
  public class FileWatcher : IFileWatcher {
    private readonly FolderWatchMetadata _folder;
    private readonly FileSystemWatcher _watcher;
    private readonly IDispatcher dispatcher;
    private readonly ILogger logger;

    public FileWatcher(FolderWatchMetadata folder, IDispatcher dispatcher, ILogger logger) {
      this.logger = logger;
      this._folder = folder;
      this._watcher = initializeWatcher();
      this.dispatcher = dispatcher;
    }

    private FileSystemWatcher initializeWatcher() {
      logger.Information("Starting: setting up FileSystemWatcher");
      var watcher = new FileSystemWatcher();
      watcher.Path = _folder.FolderPath;
      watcher.NotifyFilter = NotifyFilters.LastAccess
                                      | NotifyFilters.LastWrite
                                      | NotifyFilters.FileName
                                      | NotifyFilters.DirectoryName;
      watcher.Filter = "*.*";
      watcher.Created += newFileCreated;
      watcher.Deleted += newFileDeleted;
      logger.Information("Finished: setting up FileSystemWatcher");
      return watcher;
    }

    private void newFileDeleted(object sender, FileSystemEventArgs e) {
      var message = createDeleteFileMessageInfo(e);
      logger.Information(string.Format("File deleted: {0}", JsonConvert.SerializeObject(message)));
      dispatcher.DispatchMessage(message);
    }

    private object createDeleteFileMessageInfo(FileSystemEventArgs e) =>
      new DeleteFileMessage() {
        FilePath = e.FullPath,
        FileName = e.Name
      };

    private void newFileCreated(object sender, FileSystemEventArgs e) {
      var message = createNewFileMessageInfo(e);
      logger.Information(string.Format("File created: {0}", JsonConvert.SerializeObject(message)));
      dispatcher.DispatchMessage(message);
    }

    private object createNewFileMessageInfo(FileSystemEventArgs e) =>
      new NewFileMessage() {
        FilePath = e.FullPath,
        FileName = e.Name
      };

    public void Dispose() {
      logger.Information("Disposing FileWatcher");
      Stop();
      _watcher.Dispose();
    }

    public void Start() {
      logger.Information("Starting FileWatcher");
      _watcher.EnableRaisingEvents = true;
    }

    public void Stop() {
      logger.Information("Stopping FileWatcher");
      _watcher.EnableRaisingEvents = false;
    }
  }
}
