using System;
using System.Runtime.Serialization;

namespace FileWatcher.Core.MsgConsumers.FileLogNotification {
  public class FileLogNotificationException : ApplicationException, IEquatable<FileLogNotificationException> {
    public FileLogNotificationException(string message) : base(message) {
    }

    public FileLogNotificationException(string message, Exception innerException) : base(message, innerException) {
    }

    public FileLogNotificationException() {
    }

    protected FileLogNotificationException(SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) {
    }

    public bool Equals(FileLogNotificationException other) => string.Equals(this.Message, other?.Message);
  }
}
