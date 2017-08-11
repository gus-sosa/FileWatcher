using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace FileWatcher
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<FileFatcher>(s =>
                {
                    s.ConstructUsing(name => new FileFatcher());
                    s.WhenStarted(fw => fw.Start());
                    s.WhenStopped(fw => fw.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Program to watch file changes in a folder");
                x.SetDisplayName("FileWatcher");
                x.SetServiceName("FileWatcher");
            });
        }
    }
}
