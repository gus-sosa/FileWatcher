using Serilog;

namespace FileWatcher.Service {
  internal class FileWatcherServiceManager {
    private ILogger logger;
    private AppConfiguration appConfig;

    public FileWatcherServiceManager(ILogger logger, AppConfiguration appConfig) {
      this.logger = logger;
      this.appConfig = appConfig;
    }

    public void Start() {
      logger.Information("starting: service manager creation");
    }

    public void Stop() {
      logger.Information("finished: service manager creation");
    }
  }
}