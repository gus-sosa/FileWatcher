using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using Topshelf;

namespace FileWatcher
{
    class Program
    {
        static ILogger logger;
        static void Main(string[] args)
        {
            logger = LogManager.GetCurrentClassLogger();

            HostFactory.Run(x =>
            {
                //TODO: LOG: Log whatever error the service throws

                x.Service<FileFatcher>(s =>
                {
                    s.ConstructUsing(name => new FileFatcher(GetFolders(), logger));
                    s.WhenStarted(fw => fw.Start());
                    s.WhenStopped(fw => fw.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Program to watch file changes in a folder");
                x.SetDisplayName("FileWatcher");
                x.SetServiceName("FileWatcher");
            });
        }

        private static IEnumerable<string> GetFolders()
        {
            //TODO: LOG: Getting folders to watch

            var list = new List<string>();
            try
            {
                using (var file = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "folders.config", FileMode.Open))
                {
                    //TODO: LOG: File opened
                    using (var reader = new StreamReader(file))
                    {
                        while (true)
                        {
                            string folder = reader.ReadLine();
                            if (string.IsNullOrEmpty(folder))
                                break;
                            list.Add(folder);
                        }
                    }
                }
            }
            catch
            {
                //TODO: Log error
            }

            return list;
        }
    }
}
