using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FileWatcher.Abstracts.Contracts;
using FileWatcher.Abstracts.Domain;
using Flurl.Http;

namespace FileWatcher.Core.MsgConsumers.WinToastNotification {
  public class WinToastNotification : IMesssageConsumer<NewFileMessage>, IMesssageConsumer<DeleteFileMessage> {
    private readonly string serverHost;
    private readonly int serverPort;
    private readonly string executableServerPath;
    private Process serverProcess;

    public WinToastNotification(string executableServerPath, string serverHost = "localhost", int serverPort = 1234) {
      this.serverHost = serverHost;
      this.serverPort = serverPort;
      this.executableServerPath = executableServerPath;
      initializeServer();
#if DEBUG
      FlurlHttp.Configure(settings => {
        settings.HttpClientFactory = new ProxyHttpClientFactory();
      });
#endif
    }

    private void initializeServer() {
      this.serverProcess = new Process();
      serverProcess.StartInfo.FileName = executableServerPath;
      serverProcess.StartInfo.Arguments = $"serverHost={serverHost} serverPort={serverPort}";
#if DEBUG
      serverProcess.StartInfo.UseShellExecute = true;
#endif
      serverProcess.Start();
    }

    public async Task ConsumeMessage(NewFileMessage message) =>
      await showToast(new MessageModel() {
        Path = message.FilePath,
        Action = "new",
        Message = string.Format("File created: {0}", message.FilePath)
      });

    public async Task ConsumeMessage(DeleteFileMessage message) =>
      await showToast(new MessageModel() {
        Path = message.FilePath,
        Action = "delete",
        Message = string.Format("File deleted: {0}", message.FilePath)
      });

    private async Task showToast(MessageModel messageModel) {
      await string.Format("http://{0}:{1}", serverHost, serverPort)
    .WithHeader("Content-Type", "application/json")
    .PostJsonAsync(messageModel);
    }

    public void Dispose() {
      serverProcess.Kill();
      serverProcess.Close();
    }
  }
}
