using System.Net;
using System.Net.Http;
using Flurl.Http.Configuration;

namespace FileWatcher.Core.MsgConsumers.WinToastNotification {
  class ProxyHttpClientFactory : DefaultHttpClientFactory {
    private readonly string _address;

    public ProxyHttpClientFactory(string address = "http://localhost:8888") {
      this._address = address;
    }

    public override HttpMessageHandler CreateMessageHandler() {
      return new HttpClientHandler {
        Proxy = new WebProxy(_address),
        UseProxy = true
      };
    }
  }
}
