using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace FileWatcher.Service {
  class AppConfiguration {
    public int SizeOfLogFileInMb { get; set; }
    public int TimeToClearLogsInMinutes { get; set; }
  }
}
