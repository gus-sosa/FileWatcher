using System;
using System.Diagnostics;
using System.Threading.Tasks;
using FileWatcher.Abstracts.Contracts;
using FileWatcher.Abstracts.Domain;
using Flurl.Http;
using Serilog;

namespace FileWatcher.Core.MsgConsumers.WinToastNotification {
  public class WinToastNotification : IMesssageConsumer<NewFileMessage>, IMesssageConsumer<DeleteFileMessage> {
    private readonly string serverHost;
    private readonly int serverPort;
    private readonly string executableServerPath;
    private readonly string uri;
    private Process serverProcess;
    private ILogger logger;

    public WinToastNotification(ILogger logger, string executableServerPath, string serverHost = "localhost", int serverPort = 1234) {
      this.logger = logger;
      logger.Information("starting: initializing win toast notification");
      this.serverHost = serverHost;
      this.serverPort = serverPort;
      this.executableServerPath = executableServerPath;
      this.uri = string.Format("http://{0}:{1}", serverHost, serverPort);
      logger.Information($"uri={uri}");
      initializeServer();
#if DEBUG
      var proxy = new ProxyHttpClientFactory();
      FlurlHttp.Configure(settings => {
        settings.HttpClientFactory = proxy;
      });
      logger.Information($"using proxy at: {proxy.Address}");
#endif
      logger.Information("finished: initializing win toast notification");
    }

    private void initializeServer() {
      logger.Information("starting: initializing windows server to launch the toast");
      this.serverProcess = new Process();
      serverProcess.StartInfo.FileName = executableServerPath;
      serverProcess.StartInfo.Arguments = $"{serverHost} {serverPort}";
#if DEBUG
      logger.Information("using shell since application is running in debug mode");
      serverProcess.StartInfo.UseShellExecute = true;
#endif
      logger.Information($"starting winToastNotification.Server located at {executableServerPath} with parameters \"{serverProcess.StartInfo.Arguments}\"");
      serverProcess.Start();
      logger.Information("finished: initializing windows server to launch the toast");
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
      logger.Debug("starting: sending request");
      await uri
        .WithHeader("Content-Type", "application/json")
        .PostJsonAsync(messageModel);
      logger.Debug("finished: sending request");
    }

    public void Dispose() {
      serverProcess.Kill();
      serverProcess.Close();
    }
  }
}
