using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace FileWatcher.Service {
  class Program {
    private static ILogger _logger;
    private static AppConfiguration _appConfig;

    static void Main(string[] args) {
      _logger = createLogger();
      try {
        _appConfig = getConfiguration();
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
      _logger.Information("finished: Parsing configuration");
      return retval;
    }
  }
}
