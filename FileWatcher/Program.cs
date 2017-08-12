using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using Topshelf;
using System.Linq;

namespace FileWatcher
{
    class Program
    {
        static ILogger logger;
        static void Main(string[] args)
        {
            logger = LogManager.GetCurrentClassLogger();
            try
            {
                HostFactory.Run(x =>
                {
                    x.Service<FileWatcher>(s =>
                    {
                        s.ConstructUsing(name => new FileWatcher(GetFolders(), logger));
                        s.WhenStarted(fw => fw.Start());
                        s.WhenStopped(fw => fw.Stop());
                    });
                    x.RunAsLocalSystem();

                    x.SetDescription("Program to watch file changes in a folder");
                    x.SetDisplayName("FileWatcher");
                    x.SetServiceName("FileWatcher");
                });
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }

        private static IEnumerable<string> GetFolders()
        {
            logger.Info("Getting folders to watch");
            var list = new List<string>();
            try
            {
                using (var file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "folders.config", FileMode.Open))
                {
                    logger.Info("folders.config opened");
                    using (var reader = new StreamReader(file))
                    {
                        while (true)
                        {
                            string folder = reader.ReadLine();
                            if (string.IsNullOrEmpty(folder))
                                break;
                            list.Add(folder);
                            logger.Info($"Watch folder: {folder}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error($"Error when trying to read folders.config: {e}");
            }

            logger.Info($"Method has got all the folders to watch: {list.Aggregate("", (accumulate, current) => $"{accumulate},{current}")}");
            return list;
        }
    }
}
