using System;
using System.IO;

namespace FileWatcher.Abstracts.Domain {
  public class FolderWatchMetadata {

    private string _folderPath = string.Empty;
    public string FolderPath {
      get => _folderPath;
      set {
        if (Directory.Exists(value)) {
          _folderPath = value;
        } else {
          throw new ApplicationException(string.Format("Folder path does not exist: {0}", value));
        }
      }
    }
  }
}
