using System;
using Nancy.Hosting.Self;

namespace WinToastNotification.Server {
  static class Program {
    static void Main(string[] args) {
      Console.WriteLine($"Server host coming from console arguments={args[0]}");
      Console.WriteLine($"Port coming from console arguments={args[1]}");
      string url = $"http://{args[0]}:{args[1]}";
      var host = new NancyHost(new Uri(url));
      try {
        host.Start();
        Console.WriteLine($"Running on {url}");
        Console.ReadLine();
      } catch (Exception ex) {
        Console.WriteLine(ex.ToString());
      } finally {
        host.Dispose();
      }
    }
  }
}
