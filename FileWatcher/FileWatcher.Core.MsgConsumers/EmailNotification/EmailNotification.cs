using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileWatcher.Abstracts.Contracts;
using FileWatcher.Abstracts.Domain;
using MailKit.Net.Smtp;
using MimeKit;
using Serilog;

namespace FileWatcher.Core.MsgConsumers.EmailNotification {
  public class EmailNotification : IMesssageConsumer<NewFileMessage>, IMesssageConsumer<DeleteFileMessage> {
    private ILogger logger;
    private readonly string serverHost;
    private readonly int serverPort;
    private readonly string fromTitle;
    private readonly string fromAddress;
    private readonly List<(string name, string email)> to;

    public EmailNotification(ILogger logger, string serverHost, int serverPort, string fromTitle, string fromAddress, List<(string name, string email)> to) {
      this.logger = logger;
      this.serverHost = serverHost;
      this.serverPort = serverPort;
      this.fromTitle = fromTitle;
      this.fromAddress = fromAddress;
      this.to = to;
      validate();
    }

    private void validate() {
      EmailNotificationException error = null;
      if (serverPort <= 0) {
        error = new EmailNotificationException($"Invalid server port. It should be greater than 0. {serverPort} was provided", error);
      }
      if (string.IsNullOrEmpty(serverHost)) {
        error = new EmailNotificationException($"Invalid host. The host provided is: {serverHost}", error);
      }
      try {
        new MailboxAddress(string.Empty, fromAddress);
      } catch (Exception) {
        error = new EmailNotificationException($"Invalid from email address. Address provided = {fromAddress}", error);
      }
      foreach (var i in to) {
        try {
          new MailboxAddress(string.Empty, i.email);
        } catch (Exception) {
          error = new EmailNotificationException($"Invalid to email address. Address provided = {i.email}", error);
        }
      }

      if (error != null) {
        throw error;
      }
    }

    public async Task ConsumeMessage(NewFileMessage message) {
      //to-do: consider to use templates and template engines to generate the content
      string body = @$"
Hi,

A new file was created. Details:

File Name: {message.FileName}
Path: {message.FilePath}

Regards,
Team

";
      await sendEmail("File Watcher Notification: file created", body);
    }

    public async Task ConsumeMessage(DeleteFileMessage message) {
      //to-do: consider to use templates and template engines to generate the content
      string body = @$"
Hi,

A file was deleted. Details:

File Name: {message.FileName}
Path: {message.FilePath}

Regards,
Team

";
      await sendEmail("File Watcher Notification: file deleted", body);
    }

    public async Task sendEmail(string subject, string body) {
      logger.Debug("starting: building email");
      var message = new MimeMessage();
      message.From.Add(new MailboxAddress(fromTitle, fromAddress));
      message.To.AddRange(to.Select(i => new MailboxAddress(i.name, i.email)));
      message.Subject = subject;
      message.Body = new TextPart("plain") { Text = body };
      logger.Debug("finished: building email");
      logger.Debug("starting: sending email");
      //to-do: investigate if it is more efficient to hold this object as a field (initialize it on the constructor) and disconnect and dispose it in the dispose method
      using (var client = new SmtpClient()) {
        await client.ConnectAsync(serverHost, serverPort, false);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
      }
      logger.Debug("finished: sending email");
    }

    public void Dispose() {
    }
  }
}
