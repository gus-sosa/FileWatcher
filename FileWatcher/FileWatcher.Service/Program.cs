using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Topshelf;
using Topshelf.Runtime.DotNetCore;

namespace FileWatcher.Service {
  static class Program {
    private static ILogger _logger;
    private static AppConfiguration _appConfig;
    private static string configFilePath;

    static void Main(string[] args) {
      try {
        configFilePath = Path.GetFullPath(".\\Config.json");
        _appConfig = getConfiguration();
        _logger = createLogger();
        _logger.Information(string.Format("Configuration file's path to search: {0}", configFilePath));
        _logger.Information("starting: Parsing configuration");
        _logger.Information(string.Format("Configuration file's content: {0}", File.ReadAllText(configFilePath)));
        _logger.Information("finished: Parsing configuration");
#if SERVICE
        buildWinService();
#else
        buildServiceAndRunAsConsole();
#endif
      } catch (Exception ex) {
        _logger.Error(ex.ToString());
      }
    }

    private static void buildServiceAndRunAsConsole() {
      var service = new FileWatcherServiceManager(_logger, _appConfig);
      service.Start();
      Console.ReadLine();
      service.Stop();
    }

    private static void buildWinService() {
      _logger.Information("starting: starting service");
      var rc = HostFactory.Run(x => {
        x.Service<FileWatcherServiceManager>(s => {
          s.ConstructUsing(name => new FileWatcherServiceManager(_logger, _appConfig));
          s.WhenStarted(sm => sm.Start());
          s.WhenStopped(sm => sm.Stop());
        });
        x.RunAsLocalSystem();
        x.SetDescription(_appConfig.ServiceDescription);
        x.SetDisplayName(_appConfig.ServiceDisplayName);
        x.SetServiceName(_appConfig.ServiceName);
        x.UseEnvironmentBuilder(new Topshelf.HostConfigurators.EnvironmentBuilderFactory(c => new DotNetCoreEnvironmentBuilder(c)));
      });
      Environment.ExitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
      _logger.Information("finished: starting service");
    }

    private static ILogger createLogger() => new LoggerConfiguration().WriteTo.
#if DEBUG
      Console()
#endif
#if RELEASE
      File(path: ".", restrictedToMinimumLevel: LogEventLevel.Information, rollOnFileSizeLimit: true, fileSizeLimitBytes: 1000 * _appConfig.SizeOfLogFileInMb, retainedFileCountLimit: 10, rollingInterval: RollingInterval.Day, shared: true)
#endif
      .CreateLogger();

    private static AppConfiguration getConfiguration() =>
      new ConfigurationBuilder()
        .AddJsonFile(Path.GetFullPath(".\\Config.json"), false, false)
        .Build()
        .Get<AppConfiguration>();
  }
}
