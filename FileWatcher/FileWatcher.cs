using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications; // Notifications library
using Windows.Data.Xml.Dom;

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

        private void Renamed(object sender, RenamedEventArgs e) => NotifyChanges(Path.GetDirectoryName(e.FullPath));

        private void NotifyChanges(object sender, FileSystemEventArgs e) => NotifyChanges(Path.GetDirectoryName(e.FullPath));

        public void NotifyChanges(string folderdir)
        {
            this.Log().Info($"Notifying: {folderdir}");
            var visual = new ToastVisual()
            {
                BindingGeneric = new ToastBindingGeneric()
                {
                    Children =
                    {
                        new AdaptiveText(){ Text=Resource.Title},
                        new AdaptiveText(){ Text=folderdir }
                    }
                }
            };

            var actions = new ToastActionsCustom();

            var toastContent = new ToastContent()
            {
                Visual = visual,
                Actions = actions
            };

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(toastContent.GetContent());
            var toastNotification = new ToastNotification(xmlDoc);
            //TODO: Create a configuration file for the application and put the name of the application in a setting
            ToastNotificationManager.CreateToastNotifier("File Watcher").Show(toastNotification);
        }
    }
}