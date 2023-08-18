using System;
using TcUnit.VisualStudio.EventWatchers.EventArgs;

namespace TcUnit.VisualStudio.EventWatchers
{
    public interface ITestFilesUpdateWatcher
    {
        event EventHandler<TestFileChangedEventArgs> FileChangedEvent;
        void AddWatch(string path);
        void RemoveWatch(string path);
    }
}