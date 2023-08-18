using System;
using TcUnit.VisualStudio.EventWatchers.EventArgs;

namespace TcUnit.VisualStudio.EventWatchers
{
    public interface ITestFileAddRemoveListener
    {
        event EventHandler<TestFileChangedEventArgs> TestFileChanged;
        void StartListeningForTestFileChanges();
        void StopListeningForTestFileChanges();
    }
}