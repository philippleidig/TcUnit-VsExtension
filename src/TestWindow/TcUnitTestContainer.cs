using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using Microsoft.VisualStudio.TestWindow.Extensibility.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TcUnit.VisualStudio.TestWindow
{
    public class TcUnitTestContainer : ITestContainer
    {
        private ITestContainerDiscoverer discoverer;
        public TcUnitTestContainer(ITestContainerDiscoverer discoverer, string source, Uri executorUri)
            : this(discoverer, source, executorUri, Enumerable.Empty<Guid>())
        { }

        public TcUnitTestContainer(ITestContainerDiscoverer discoverer, string source, Uri executorUri, IEnumerable<Guid> debugEngines)
        {
            this.Source = source;
            this.ExecutorUri = executorUri;
            this.DebugEngines = debugEngines;
            this.discoverer = discoverer;
            this.TargetFramework = FrameworkVersion.None;
            this.TargetPlatform = Architecture.AnyCPU;
            this.timeStamp = GetTimeStamp();
        }

        private TcUnitTestContainer(TcUnitTestContainer copy)
            : this(copy.discoverer, copy.Source, copy.ExecutorUri)
        {
            this.timeStamp = copy.timeStamp;
        }

        private DateTime GetTimeStamp()
        {
            if (!string.IsNullOrEmpty(this.Source) && File.Exists(this.Source))
            {
                var directory = new DirectoryInfo(Path.GetDirectoryName(this.Source));
                return directory.GetFiles("*.TcPOU", SearchOption.AllDirectories)
                         .OrderByDescending(f => f.LastWriteTime)
                         .First().LastWriteTime;
            }
            else
            {
                return DateTime.MinValue;
            }

        }

        private readonly DateTime timeStamp;

        public string Source { get; set; }
        public Uri ExecutorUri { get; set; }
        public IEnumerable<Guid> DebugEngines { get; set; }
        public FrameworkVersion TargetFramework { get; set; }
        public Architecture TargetPlatform { get; set; }
        public override string ToString()
        {
            return this.ExecutorUri.ToString() + "/" + this.Source;
        }
        public IDeploymentData DeployAppContainer() { return null; }
        public bool IsAppContainerTestContainer { get { return false; } }
        public ITestContainerDiscoverer Discoverer { get { return discoverer; } }

        public int CompareTo(ITestContainer other)
        {
            var testContainer = other as TcUnitTestContainer;
            if (testContainer == null)
            {
                return -1;
            }

            var result = string.Compare(this.Source, testContainer.Source, StringComparison.OrdinalIgnoreCase);
            if (result != 0)
            {
                return result;
            }

            return this.timeStamp.CompareTo(testContainer.timeStamp);
        }

        public ITestContainer Snapshot()
        {
            return new TcUnitTestContainer(this);
        }
    }

}
