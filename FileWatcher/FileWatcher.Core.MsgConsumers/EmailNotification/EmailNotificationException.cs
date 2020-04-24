using System;
using System.Runtime.Serialization;

namespace FileWatcher.Core.MsgConsumers.EmailNotification {
  public class EmailNotificationException : ApplicationException, IEquatable<EmailNotificationException> {
    public EmailNotificationException(string message) : base(message) {
    }

    public EmailNotificationException(string message, Exception innerException) : base(message, innerException) {
    }

    public EmailNotificationException() {
    }

    protected EmailNotificationException(SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) {
    }

    public bool Equals(EmailNotificationException other) => string.Equals(this.Message, other?.Message);
  }
}
