using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using Topshelf;
using Hangfire;
using Hangfire.MemoryStorage;

namespace FileWatcher
{
    /// <summary>
    /// Start point of the program
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            LoggingExtensions.Logging.Log.InitializeWith<LoggingExtensions.NLog.NLogLog>();
            try
            {
                //Setting up HangFire for the task (cleaning the logs)
                GlobalConfiguration.Configuration.UseMemoryStorage();
                var hangFireServer = new BackgroundJobServer(new BackgroundJobServerOptions() { WorkerCount = 1 });
                RecurringJob.AddOrUpdate(() => CleanLogs(), Cron.MinuteInterval(Settings.Default.MinutesIntervalToCleanLogs));

                HostFactory.Run(x =>
                {
                    x.Service<FileWatcher>(s =>
                    {
                        s.ConstructUsing(name => new FileWatcher(GetFolders()));
                        s.WhenStarted(fw => fw.Start());
                        s.WhenStopped(fw =>
                        {
                            fw.Stop();
                            hangFireServer.Dispose();
                        });
                    });
                    x.RunAsLocalSystem();

                    x.SetDescription(Settings.Default.Description);
                    x.SetDisplayName(Settings.Default.DisplayName);
                    x.SetServiceName(Settings.Default.ServiceName);
                });
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
            }
        }

        public static void CleanLogs()
        {
            var logsFolderDir = Path.GetFullPath(Settings.Default.LogsFolderDir);
            foreach (string filePath in Directory.EnumerateFiles(logsFolderDir))
                File.Delete(filePath);
        }

        /// <summary>
        /// Get the paths of folders to watch from the folders config file
        /// </summary>
        /// <exception cref="Exception">
        /// It may throw this exception if it cannot find the folders config file
        /// </exception>
        /// <returns>
        /// List of the paths to watch
        /// </returns>
        public static IEnumerable<string> GetFolders()
        {
            LogManager.GetCurrentClassLogger().Info("Getting folders to watch");
            var list = new List<string>();
            string folderConfig = Settings.Default.NameFoldersConfigFile;
            try
            {
                using (var file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + folderConfig, FileMode.Open))
                {
                    LogManager.GetCurrentClassLogger().Info($"{folderConfig} opened");
                    using (var reader = new StreamReader(file))
                    {
                        while (true)
                        {
                            string folder = reader.ReadLine();
                            if (string.IsNullOrEmpty(folder))
                                break;
                            list.Add(folder);
                            LogManager.GetCurrentClassLogger().Info($"Watch folder: {folder}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error($"Error when trying to read {folderConfig}: {e}");
            }

            LogManager.GetCurrentClassLogger().Info("Method has got all the folders to watch");
            return list;
        }
    }
}
