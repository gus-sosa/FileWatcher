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
            folders = folders?.Where(f => Directory.Exists(f));
            if (folders == null || folders.Count() == 0)
                throw new InvalidOperationException("There are no folders to watch");

            Folders = folders;
        }

        internal IList<FileSystemWatcher> FileSystemWatcher { get; private set; } = new List<FileSystemWatcher>();
        internal IEnumerable<string> Folders { get; private set; }

        public void Start()
        {
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
        }

        internal bool Stop()
        {
            throw new NotImplementedException();
        }

        private void NotifyChanges(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}