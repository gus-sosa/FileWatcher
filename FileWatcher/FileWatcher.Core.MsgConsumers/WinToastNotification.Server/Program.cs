using System;
using Nancy.Hosting.Self;

namespace WinToastNotification.Server {
  static class Program {
    static void Main(string[] args) {
      string host = args.Length > 1 ? args[0] : "localhost";
      string port = args.Length > 2 ? args[1] : "1234";
      Console.WriteLine($"Server host coming from console arguments={host}");
      Console.WriteLine($"Port coming from console arguments={port}");
      string url = $"http://{host}:{port}";
      var nancyHost = new NancyHost(new Uri(url));
      try {
        nancyHost.Start();
        Console.WriteLine($"Running on {url}");
        Console.ReadLine();
      } catch (Exception ex) {
        Console.WriteLine(ex.ToString());
      } finally {
        nancyHost.Dispose();
      }
    }
  }
}
