using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using Topshelf;

namespace FileWatcher
{
    //TODO: Add documentation (in XML document and in GitHub)
    //TODO: Commenting classes and methods
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

                    //TODO: Put in a setting files these values
                    x.SetDescription("Program to watch file changes in a folder");
                    x.SetDisplayName("FileWatcher");
                    x.SetServiceName("FileWatcher");
                });
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error(e);
            }
        }

        private static IEnumerable<string> GetFolders()
        {
            LogManager.GetCurrentClassLogger().Info("Getting folders to watch");
            var list = new List<string>();
            try
            {
                using (var file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "folders.config", FileMode.Open))
                {
                    LogManager.GetCurrentClassLogger().Info("folders.config opened");
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
                LogManager.GetCurrentClassLogger().Error($"Error when trying to read folders.config: {e}");
            }

            LogManager.GetCurrentClassLogger().Info("Method has got all the folders to watch");
            return list;
        }
    }
}
