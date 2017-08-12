using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileWatcher
{
    internal class FileWatcher
    {
        public FileWatcher(IEnumerable<string> folders, ILogger logger)
        {
            //TODO: LOG: Building service and serialize list of folders
            Logger = logger;
            logger.Info($"Building service with list of folders: {folders.Aggregate("", (acumulate, current) => $"{acumulate},{current}")}");
            folders = folders?.Where(f => Directory.Exists(f));
            if (folders == null || folders.Count() == 0)
                throw new InvalidOperationException("There are no folders to watch");

            Folders = folders;
            logger.Info("Service built successfully");
        }

        internal IList<FileSystemWatcher> FileSystemWatcher { get; private set; } = new List<FileSystemWatcher>();
        internal IEnumerable<string> Folders { get; private set; }
        internal ILogger Logger;

        public void Start()
        {
            Logger.Info("Starting service");
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
            Logger.Info("Service started successfully");
        }

        internal bool Stop()
        {
            Logger.Info("Stopping service");
            try
            {
                foreach (var fw in FileSystemWatcher)
                    fw.EnableRaisingEvents = false;
            }
            catch (Exception e)
            {
                Logger.Error($"Error stopping service: {e}");
                return false;
            }
            Logger.Info("Service stopped successfully");
            return true;
        }

        private void Renamed(object sender, RenamedEventArgs e) => NotifyChanges(Path.GetPathRoot(e.FullPath));

        private void NotifyChanges(object sender, FileSystemEventArgs e) => NotifyChanges(Path.GetPathRoot(e.FullPath));

        public void NotifyChanges(string folderdir)
        {
            Logger.Info($"Notifying: {folderdir}");
            throw new NotImplementedException();
        }
    }
}