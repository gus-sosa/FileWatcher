using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;
using Topshelf;
using Topshelf.Runtime.DotNetCore;

namespace FileWatcher.Service {
  class Program {
    private static ILogger _logger;
    private static AppConfiguration _appConfig;

    static void Main(string[] args) {
      _logger = createLogger();
      try {
        _appConfig = getConfiguration();
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
      } catch (Exception ex) {
        _logger.Error(ex.ToString());
      }
    }

    private static ILogger createLogger() => new LoggerConfiguration().WriteTo.Console().CreateLogger();

    private static AppConfiguration getConfiguration() {
      var configurationFilePath = Path.GetFullPath(".\\Config.json");
      _logger.Information(string.Format("Configuration file's path to search: {0}", configurationFilePath));
      _logger.Information("starting: Parsing configuration");
      var configurationBuilder = new ConfigurationBuilder();
      var retval = configurationBuilder
        .AddJsonFile(configurationFilePath, false, false)
        .Build()
        .Get<AppConfiguration>();
      _logger.Information(string.Format("Configuration file's content: {0}", File.ReadAllText(configurationFilePath)));
      _logger.Information("finished: Parsing configuration");
      return retval;
    }
  }
}
