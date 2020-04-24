using System;
using System.IO;
using System.Threading.Tasks;
using FileWatcher.Abstracts.Contracts;
using FileWatcher.Abstracts.Domain;
using Newtonsoft.Json;

namespace FileWatcher.Core.MsgConsumers.FileLogNotification {
  public class FileLogNotification : IMesssageConsumer<NewFileMessage>, IMesssageConsumer<DeleteFileMessage> {
    private StreamWriter logFile;

    class LogFile {
      public DateTime Timestamp { get; set; } = DateTime.Now;
      public string Action { get; set; }
      public FileMessage Message { get; set; }
    }

    public FileLogNotification(string folderPath, string fileName) {
      if (!Directory.Exists(folderPath)) {
        throw new FileLogNotificationException($"Invalid folder: {folderPath}");
      }
      this.logFile = File.CreateText(Path.Combine(folderPath, fileName));
    }

    public async Task ConsumeMessage(DeleteFileMessage message) =>
      await writeMessage(JsonConvert.SerializeObject(new LogFile() {
        Action = "delete",
        Message = message
      }));

    public async Task ConsumeMessage(NewFileMessage message) =>
      await writeMessage(JsonConvert.SerializeObject(new LogFile() {
        Action = "new",
        Message = message
      }));

    private async Task writeMessage(string content) => await logFile.WriteLineAsync(content).ContinueWith(_ => logFile.FlushAsync());

    public void Dispose() => logFile.Dispose();
  }
}
