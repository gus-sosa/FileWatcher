using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using Topshelf;

namespace FileWatcher
{
    //TODO: Add documentation (in XML document and in GitHub)
    class Program
    {
        static void Main(string[] args)
        {
            LoggingExtensions.Logging.Log.InitializeWith<LoggingExtensions.NLog.NLogLog>();
            try
            {
                HostFactory.Run(x =>
                {
                    x.Service<FileWatcher>(s =>
                    {
                        s.ConstructUsing(name => new FileWatcher(GetFolders()));
                        s.WhenStarted(fw => fw.Start());
                        s.WhenStopped(fw => fw.Stop());
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

        /// <summary>
        /// Get the paths of folders to watch from the folders config file
        /// </summary>
        /// <exception cref="Exception">
        /// It may throw this exception if it cannot find the folders config file
        /// </exception>
        /// <returns>
        /// List of the paths to watch
        /// </returns>
        private static IEnumerable<string> GetFolders()
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
