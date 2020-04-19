using System;
using System.Net;
using System.Net.Http;
using Flurl.Http.Configuration;

namespace FileWatcher.Core.MsgConsumers.WinToastNotification {
  class ProxyHttpClientFactory : DefaultHttpClientFactory {
    private string _address;
    public string Address {
      get => _address;
      set {
        if (Uri.IsWellFormedUriString(value, UriKind.Absolute)) {
          _address = value;
        }
      }
    }
    public ProxyHttpClientFactory() {
      Address = "http://localhost:8888";
    }

    public override HttpMessageHandler CreateMessageHandler() {
      return new HttpClientHandler {
        Proxy = new WebProxy(Address),
        UseProxy = true
      };
    }
  }
}
