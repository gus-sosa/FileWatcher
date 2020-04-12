using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace FileWatcher.Service {
  class Program {
    static void Main(string[] args) {
      var _appConfig = getConfiguration();
    }

    private static AppConfiguration getConfiguration() {
      var configurationFilePath = Path.GetFullPath(".\\Config.json");
      Console.WriteLine($"Config file path to search: {configurationFilePath}");
      Console.WriteLine("starting: Parsing configuration");
      var configurationBuilder = new ConfigurationBuilder();
      var retval = configurationBuilder
        .AddJsonFile(configurationFilePath, false, false)
        .Build()
        .Get<AppConfiguration>();
      Console.WriteLine("finished: Parsing configuration");
      return retval;
    }
  }
}
