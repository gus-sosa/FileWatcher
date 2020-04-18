using FileWatcher.Abstracts.Definitions;

namespace FileWatcher.Abstracts.Domain {
  public class DeleteFileMessage : FileMessage {
    public override MessageType GetMessageType() => MessageType.Removed;
  }
}
