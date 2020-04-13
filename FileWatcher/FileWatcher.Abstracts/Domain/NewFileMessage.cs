using FileWatcher.Abstracts.Definitions;

namespace FileWatcher.Abstracts.Domain {
  public class NewFileMessage : FileMessage {
    public override MessageType GetMessageType() => MessageType.New;
  }
}
