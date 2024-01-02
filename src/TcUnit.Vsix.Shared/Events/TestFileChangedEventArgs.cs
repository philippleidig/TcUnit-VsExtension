namespace TcUnit.VisualStudio.EventWatchers.EventArgs
{
    public enum TestFileChangedReason
    {
        None,
        Added,
        Removed,
        Changed,
    }

    public class TestFileChangedEventArgs : System.EventArgs
    {
        public string TestContainer { get; private set; }
        public string File { get; private set; }
        public TestFileChangedReason ChangedReason { get; private set; }

        public TestFileChangedEventArgs(string container, string file, TestFileChangedReason reason)
        {
            TestContainer = container;
            File = file;
            ChangedReason = reason;
        }
    }
}