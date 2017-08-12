using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileWatcher
{
    internal class FileWatcher
    {
        public FileWatcher(IEnumerable<string> folders)
        {
            this.Log().Info($"Building service with list of folders: {folders.Aggregate("", (acumulate, current) => $"{acumulate},{current}")}");
            folders = folders?.Where(f => Directory.Exists(f));
            if (folders == null || folders.Count() == 0)
                throw new InvalidOperationException("There are no folders to watch");

            Folders = folders;
            this.Log().Info("Service built successfully");
        }

        internal IList<FileSystemWatcher> FileSystemWatcher { get; private set; } = new List<FileSystemWatcher>();
        internal IEnumerable<string> Folders { get; private set; }

        public void Start()
        {
            this.Log().Info("Starting service");
            foreach (string folderDir in Folders)
            {
                var fw = new FileSystemWatcher();
                fw.Path = folderDir;
                fw.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime
                    | NotifyFilters.DirectoryName | NotifyFilters.FileName
                    | NotifyFilters.LastAccess | NotifyFilters.LastWrite
                    | NotifyFilters.Security | NotifyFilters.Size;

                fw.Filter = "*.*";
                var notifyChanges = new FileSystemEventHandler(NotifyChanges);
                fw.Changed += notifyChanges;
                fw.Deleted += notifyChanges;
                fw.Renamed += Renamed; ;
                fw.Changed += notifyChanges;
                fw.EnableRaisingEvents = true;

                FileSystemWatcher.Add(fw);
            }
            this.Log().Info("Service started successfully");
        }

        internal bool Stop()
        {
            this.Log().Info("Stopping service");
            try
            {
                foreach (var fw in FileSystemWatcher)
                    fw.EnableRaisingEvents = false;
            }
            catch (Exception e)
            {
                this.Log().Error($"Error stopping service: {e}");
                return false;
            }
            this.Log().Info("Service stopped successfully");
            return true;
        }

        private void Renamed(object sender, RenamedEventArgs e) => NotifyChanges(Path.GetPathRoot(e.FullPath));

        private void NotifyChanges(object sender, FileSystemEventArgs e) => NotifyChanges(Path.GetPathRoot(e.FullPath));

        public void NotifyChanges(string folderdir)
        {
            this.Log().Info($"Notifying: {folderdir}");
            throw new NotImplementedException();
        }
    }
}