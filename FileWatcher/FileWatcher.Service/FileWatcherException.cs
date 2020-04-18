using System;

namespace FileWatcher.Service {
  public class FileWatcherException : ApplicationException, IEquatable<FileWatcherException> {
    public FileWatcherException(string message) : base(message) {
    }

    public FileWatcherException(string message, Exception innerException) : base(message, innerException) {
    }

    public FileWatcherException() {
    }

    protected FileWatcherException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) {
    }

    public bool Equals(FileWatcherException other) => string.Equals(this.Message, other?.Message);
  }
}
