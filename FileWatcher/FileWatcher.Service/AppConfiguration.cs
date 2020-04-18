using System.Collections.Generic;

namespace FileWatcher.Service {
  class AppConfiguration {
    public int SizeOfLogFileInMb { get; set; }
    public int TimeToClearLogsInMinutes { get; set; }
    public string ServiceDescription { get; set; }
    public string ServiceDisplayName { get; set; }
    public string ServiceName { get; set; }

    public ICollection<FileConfig> FoldersToWatch { get; set; }
    public string WindowsToastServerPath { get; set; }

    public class FileConfig {
      public string FolderPath { get; set; }
    }
  }
}
