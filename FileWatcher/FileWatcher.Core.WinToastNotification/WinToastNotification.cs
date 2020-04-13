using System;
using FileWatcher.Abstracts.Contracts;
using FileWatcher.Abstracts.Domain;

namespace FileWatcher.Core.WinToastNotification {
  public class WinToastNotification : IMesssageConsumer<NewFileMessage>, IMesssageConsumer<DeleteFileMessage> {
    public void ConsumeMessage(DeleteFileMessage message) {
      throw new NotImplementedException();
    }

    public void ConsumeMessage(NewFileMessage message) {
      throw new NotImplementedException();
    }

    public void Dispose() {
      throw new NotImplementedException();
    }
  }
}
