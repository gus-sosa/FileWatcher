using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Topshelf;
using Topshelf.Runtime.DotNetCore;

namespace FileWatcher.Service {
  static class Program {
    private const string APP_CONFIG = "AppConfig";
    private const string LOG_FOLDER = "LogFolder";
    private static ILogger _logger;
    private static AppConfiguration _appConfig;
    private static string configFilePath;
    private static string logFolder;

    static void Main(string[] args) {
      //System.Diagnostics.Debugger.Launch();
      configFilePath = Path.GetFullPath(".\\Config.json");
      logFolder = Path.GetFullPath(".\\logfolder\\log");
      if ((args.Length == 0 || args[0] != "start") && args.Length != 4) {
        createLogger();
        buildConfiguration();
      }
      try {
        var rc = HostFactory.Run(x => {
          x.Service<FileWatcherServiceManager>(s => {
            if (_logger == null) {
              createLoggerFromRegistry();
            }
            if (_appConfig == null) {
              buildAppConfigFromRegistry();
            }
            s.ConstructUsing(name => new FileWatcherServiceManager(_logger, _appConfig));
            s.WhenStarted((FileWatcherServiceManager sm, HostControl h) => {
              h.RequestAdditionalTime(TimeSpan.FromMinutes(120));
              _logger.Information("starting: starting service");
              sm.Start();
              _logger.Information("finished: starting service");
              return true;
            });
            s.WhenStopped(sm => {
              _logger.Information("starting: stopping service");
              sm.Stop();
              _logger.Information("finished: stopping service");
            });
          });
          x.AfterInstall(() => cacheConfigurationInRegistry());
          x.UseEnvironmentBuilder(new Topshelf.HostConfigurators.EnvironmentBuilderFactory(c => new DotNetCoreEnvironmentBuilder(c)));
          x.OnException(x => _logger.Error(x.ToString()));
          x.SetDescription(_appConfig.ServiceDescription);
          x.SetDisplayName(_appConfig.ServiceDisplayName);
          x.SetServiceName(_appConfig.ServiceName);
          x.RunAsLocalSystem();
        });
        Environment.ExitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
      } catch (Exception ex) {
        _logger.Error(ex.ToString());
      }
    }

    private static void cacheConfigurationInRegistry() {
      if (_appConfig == null || logFolder == null) {
        return;
      }
      RegistryKey reg = getRegistry();
      reg.SetValue(APP_CONFIG, JsonConvert.SerializeObject(_appConfig), RegistryValueKind.String);
      reg.SetValue(LOG_FOLDER, logFolder);
    }

    private static RegistryKey getRegistry() {
      var soft = Registry.LocalMachine.OpenSubKey("Software", true);
      if (soft == null) {
        soft = Registry.CurrentUser.CreateSubKey("Software", true);
      }
      var serv = soft.OpenSubKey("FileWatcher.Service", true);
      if (serv == null) {
        serv = soft.CreateSubKey("FileWatcher.Service", true);
      }
      var reg = serv.OpenSubKey(Environment.Version.ToString(), true);
      if (reg == null) {
        reg = serv.CreateSubKey(Environment.Version.ToString(), true);
      }

      return reg;
    }

    private static void buildAppConfigFromRegistry() {
      if (_appConfig != null) {
        return;
      }
      var reg = getRegistry();
      var temporalFilePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, $"FileWatcher.Service.{Guid.NewGuid().ToString()}.config"));
      var appConfigContent = reg.GetValue(APP_CONFIG).ToString();
      File.WriteAllText(temporalFilePath, appConfigContent);
      configFilePath = temporalFilePath;
      buildConfiguration();
      File.Delete(temporalFilePath);
    }

    private static void createLoggerFromRegistry() {
      if (_logger != null) {
        return;
      }
      var reg = getRegistry();
      var o = reg.GetValue(LOG_FOLDER);
      logFolder = o.ToString();
      createLogger();
    }

    private static void createLogger() =>
      _logger = new LoggerConfiguration().WriteTo.
#if DEBUG && !SERVICE
      Console()
#else
      File(path: logFolder, restrictedToMinimumLevel: LogEventLevel.Information, rollOnFileSizeLimit: true, fileSizeLimitBytes: 10000000, retainedFileCountLimit: 10, rollingInterval: RollingInterval.Day, shared: false, flushToDiskInterval: TimeSpan.FromSeconds(10), buffered: true)
#endif
      .CreateLogger();

    private static void buildConfiguration() {
      _logger.Information(string.Format("Configuration file's path to search: {0}", configFilePath));
      _logger.Information("starting: Parsing configuration");
      _logger.Information(string.Format("Configuration file's content: {0}", File.ReadAllText(configFilePath)));
      _appConfig = new ConfigurationBuilder()
          .AddJsonFile(configFilePath, false, false)
          .Build()
          .Get<AppConfiguration>();
      _logger.Information("finished: Parsing configuration");
    }
  }
}
