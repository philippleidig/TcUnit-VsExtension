using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using TcUnit.VisualStudio.EventWatchers.EventArgs;
using System.Linq;

namespace TcUnit.VisualStudio.EventWatchers
{
    [Export(typeof(ITestFilesUpdateWatcher))]
    public class TestFilesUpdateWatcher : IDisposable, ITestFilesUpdateWatcher
    {
        private class FileWatcherInfo
        {
            public FileWatcherInfo(FileSystemWatcher watcher)
            {
                Watcher = watcher;
                LastEventTime = DateTime.MinValue;
            }

            public FileSystemWatcher Watcher { get; set; }
            public DateTime LastEventTime { get; set; }
        }

        private IDictionary<string, FileWatcherInfo> fileWatchers;
        public event EventHandler<TestFileChangedEventArgs> FileChangedEvent;

        public TestFilesUpdateWatcher()
        {
            fileWatchers = new Dictionary<string, FileWatcherInfo>(StringComparer.OrdinalIgnoreCase);
        }

        public void AddWatch(string path)
        {

            if (!String.IsNullOrEmpty(path))
            {

                var directoryName = Path.GetDirectoryName(path);
                if (!Directory.Exists(directoryName))
                    return;

                var fileName = Path.GetFileName(path);

                FileWatcherInfo watcherInfo;
                if (!fileWatchers.TryGetValue(path, out watcherInfo))
                {
                    var watcher = new FileSystemWatcher(directoryName); 

                    watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                                         | NotifyFilters.FileName | NotifyFilters.DirectoryName;

                    //watcher.Filter = "*.TcPOU";
                    watcher.IncludeSubdirectories = true;

                    watcherInfo = new FileWatcherInfo(watcher);
                    fileWatchers.Add(path, watcherInfo);

                    watcherInfo.Watcher.Changed += OnChanged;
                    watcherInfo.Watcher.Created += OnChanged;
                    watcherInfo.Watcher.Deleted += OnChanged;
                    watcherInfo.Watcher.Renamed += OnChanged;
                    watcherInfo.Watcher.EnableRaisingEvents = true;
                }
            }
        }

        public void RemoveWatch(string path)
        {

            if (!String.IsNullOrEmpty(path))
            {
                FileWatcherInfo watcherInfo;
                if (fileWatchers.TryGetValue(path, out watcherInfo))
                {
                    watcherInfo.Watcher.EnableRaisingEvents = false;

                    fileWatchers.Remove(path);
                    watcherInfo.Watcher.Changed -= OnChanged;
                    watcherInfo.Watcher.Created -= OnChanged;
                    watcherInfo.Watcher.Deleted -= OnChanged;
                    watcherInfo.Watcher.Renamed -= OnChanged;
                    watcherInfo.Watcher.Dispose();
                    watcherInfo.Watcher = null;
                }
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            FileWatcherInfo watcherInfo;

            var watcher = (FileSystemWatcher)sender;

            var testContainer = Directory.GetFiles(watcher.Path, "*.tsproj", SearchOption.TopDirectoryOnly).FirstOrDefault();

            if(e.FullPath.Contains(watcher.Path) && !string.IsNullOrEmpty(testContainer) )
            {
                if (FileChangedEvent != null && fileWatchers.TryGetValue(testContainer, out watcherInfo))
                {
                    var writeTime = File.GetLastWriteTime(e.FullPath);
                    // Only fire update if enough time has passed since last update to prevent duplicate events
                    if (writeTime.Subtract(watcherInfo.LastEventTime).TotalMilliseconds > 500)
                    {
                        watcherInfo.LastEventTime = writeTime;
                        FileChangedEvent(sender, new TestFileChangedEventArgs(testContainer, e.FullPath, TestFileChangedReason.Changed));
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && fileWatchers != null)
            {
                foreach (var fileWatcher in fileWatchers.Values)
                {
                    if (fileWatcher != null && fileWatcher.Watcher != null)
                    {
                        fileWatcher.Watcher.Changed -= OnChanged;
                        fileWatcher.Watcher.Dispose();
                    }
                }

                fileWatchers.Clear();
                fileWatchers = null;
            }
        }
    }
}