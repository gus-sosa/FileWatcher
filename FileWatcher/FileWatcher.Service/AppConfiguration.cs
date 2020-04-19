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
    public string EmailServer { get; set; }
    public int EmailServerPort { get; set; }
    public string EmailFromTitle { get; set; }
    public string EmailFromAddress { get; set; }
    public ICollection<ToEmail> EmailTos { get; set; }
    public string FileLog_FolderPath { get; set; }
    public string FileLog_FileName { get; set; }

    public class FileConfig {
      public string FolderPath { get; set; }
    }

    public class ToEmail {
      public string Name { get; set; }
      public string Email { get; set; }
    }
  }
}
