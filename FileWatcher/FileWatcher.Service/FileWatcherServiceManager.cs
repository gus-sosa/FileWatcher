using System;
using System.Collections.Generic;
using System.Linq;
using FileWatcher.Abstracts.Contracts;
using FileWatcher.Abstracts.Domain;
using Newtonsoft.Json;
using Serilog;
using FileWatcherProcess = FileWatcher.Core.FileWatcher;

namespace FileWatcher.Service {
  internal class FileWatcherServiceManager {
    private ILogger logger;
    private AppConfiguration appConfig;
    private IFileWatcher[] fileWatchers;

    public FileWatcherServiceManager(ILogger logger, AppConfiguration appConfig) {
      this.logger = logger;
      this.appConfig = appConfig;
    }

    public void Start() {
      logger.Information("starting: service manager creation");
      List<FolderWatchMetadata> foldersToWatch = getFoldersToWatch();
      logger.Information("List of folders that application is going to watch: {0}", foldersToWatch.Select(i => i.FolderPath).ToArray());
      if (foldersToWatch.Count == 0) {
        throw new ApplicationException("Stopping service since there are no valid folders to watch");
      }
      fileWatchers = initializeFileWatchers(foldersToWatch).ToArray();
      logger.Information(string.Format("number of active file watcher: {0}", fileWatchers.Length));
      if (fileWatchers.Length == 0) {
        throw new ApplicationException("Stopping service since application was not able to initialize any file watcher");
      }
    }

    private List<IFileWatcher> initializeFileWatchers(List<FolderWatchMetadata> foldersToWatch) {
      var l = new List<IFileWatcher>();
      foreach (var item in foldersToWatch) {
        try {
          var fw = new FileWatcherProcess(item);
          fw.Start();
          l.Add(fw);
        } catch (Exception ex) {
          logger.Error(string.Format("an error occurred when starting to watch folder. exception raised: {0}", ex.ToString()));
        }
      }
      return l;
    }

    private List<FolderWatchMetadata> getFoldersToWatch() {
      var foldersToWatch = new List<FolderWatchMetadata>();
      bool flag = true;
      foreach (var fw in appConfig.FoldersToWatch) {
        flag = true;
        try {
          foldersToWatch.Add(new FolderWatchMetadata() { FolderPath = fw.FolderPath });
        } catch (ApplicationException ex) {
          flag = false;
          logger.Error(ex.ToString());
        } catch (Exception ex) {
          flag = false;
          logger.Fatal(ex.ToString());
        } finally {
          if (!flag) {
            logger.Error("Application is not able to watcher folder with following configuration: {0}", JsonConvert.SerializeObject(fw));
          }
        }
      }
      return foldersToWatch;
    }

    public void Stop() {
      foreach (var fw in fileWatchers) {
        try {
          fw.Stop();
          fw.Dispose();
        } catch (Exception ex) {
          logger.Error(string.Format("an error occurred when stopping file watcher process. exception raised: {0}", ex.ToString()));
        }
      }
      logger.Information("finished: service manager creation");
    }
  }
}