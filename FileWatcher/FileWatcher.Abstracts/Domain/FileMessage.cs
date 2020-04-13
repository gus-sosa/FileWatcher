using FileWatcher.Abstracts.Definitions;

namespace FileWatcher.Abstracts.Domain {
  public abstract class FileMessage {
    public string FilePath { get; set; }
    public string FileName { get; set; }

    public abstract MessageType GetMessageType();
  }
}
