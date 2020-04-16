using System;
using Nancy;
using Nancy.ModelBinding;
using Newtonsoft.Json;

namespace FileWatcher.Core.MsgConsumers.WinToastNotification.Server {
  public class WinToastNotificationServer : NancyModule {
    public WinToastNotificationServer() {
      Post("/", _ => {
        showToast(this.BindTo(new ToastMessageModel()));
        return string.Empty;
      });
    }

    private void showToast(ToastMessageModel input) => Console.WriteLine(JsonConvert.SerializeObject(input));
  }
}
