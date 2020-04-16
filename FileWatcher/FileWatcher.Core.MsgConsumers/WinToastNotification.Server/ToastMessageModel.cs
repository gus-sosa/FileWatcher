namespace FileWatcher.Core.MsgConsumers.WinToastNotification.Server {
  public class ToastMessageModel {
    public string Action { get; set; }
    public string Message { get; set; }
    public string Path { get; set; }
  }
}
