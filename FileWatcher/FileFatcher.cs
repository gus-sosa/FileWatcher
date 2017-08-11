using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileWatcher
{
    internal class FileFatcher
    {
        public FileFatcher(IEnumerable<string> folders)
        {
            //TODO: LOG: Building service and serialize list of folders
            folders = folders?.Where(f => Directory.Exists(f));
            if (folders == null || folders.Count() == 0)
                throw new InvalidOperationException("There are no folders to watch");

            Folders = folders;
            //TODO: LOG: Service builded successfully
        }

        internal IList<FileSystemWatcher> FileSystemWatcher { get; private set; } = new List<FileSystemWatcher>();
        internal IEnumerable<string> Folders { get; private set; }

        public void Start()
        {
            //TODO: LOG: Starting service
            foreach (string folderDir in Folders)
            {
                var fw = new FileSystemWatcher();
                fw.Path = folderDir;
                fw.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime
                    | NotifyFilters.DirectoryName | NotifyFilters.FileName
                    | NotifyFilters.LastAccess | NotifyFilters.LastWrite
                    | NotifyFilters.Security | NotifyFilters.Size;

                fw.Filter = "*.*";
                fw.Changed += new FileSystemEventHandler(NotifyChanges);
                fw.EnableRaisingEvents = true;

                FileSystemWatcher.Add(fw);
            }
            //TODO: LOG: Service started successfully
        }

        internal bool Stop()
        {
            //TODO: LOG: Stopping service
            throw new NotImplementedException();
            //TODO: LOG: Service stopped
        }

        private void NotifyChanges(object sender, FileSystemEventArgs e)
        {
            //TODO: LOG: Log notification and path of the folder
            throw new NotImplementedException();
        }
    }
}