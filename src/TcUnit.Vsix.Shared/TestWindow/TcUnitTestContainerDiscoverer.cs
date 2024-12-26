using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TestWindow.Extensibility;
using TcUnit.VisualStudio.EventWatchers;
using TcUnit.VisualStudio.EventWatchers.EventArgs;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using TcUnit.EventWatchers;

namespace TcUnit.VisualStudio.TestWindow
{

    [Export(typeof(ITestContainerDiscoverer))]
    public class TcUnitTestContainerDiscoverer : ITestContainerDiscoverer
    {
        public const string ExecutorUriString = "executor://TcUnitTestExecutor";

        public event EventHandler TestContainersUpdated;
        private readonly IServiceProvider serviceProvider;
        private readonly EnvDTE.DTE dte;
        private ILogger logger;
        private ISolutionEventsListener solutionListener;
        private ITestFilesUpdateWatcher testFilesUpdateWatcher;
        private ITestFileAddRemoveListener testFilesAddRemoveListener;
        private bool initialContainerSearch;
        private readonly List<ITestContainer> cachedContainers;
        protected string FileExtension { get => ".tsproj"; }
        public Uri ExecutorUri { get { return new System.Uri(ExecutorUriString); } }
        public IEnumerable<ITestContainer> TestContainers { get { return GetTestContainers(); } }

        [ImportingConstructor]
        public TcUnitTestContainerDiscoverer(
            [Import(typeof(SVsServiceProvider))] IServiceProvider serviceProvider,
            ILogger logger,
            ISolutionEventsListener solutionListener,
            ITestFilesUpdateWatcher testFilesUpdateWatcher,
            ITestFileAddRemoveListener testFilesAddRemoveListener)
        {
			ThreadHelper.ThrowIfNotOnUIThread();

			initialContainerSearch = true;
            cachedContainers = new List<ITestContainer>();
            this.serviceProvider = serviceProvider;
            this.logger = logger;
            this.solutionListener = solutionListener;
            this.testFilesUpdateWatcher = testFilesUpdateWatcher;
            this.testFilesAddRemoveListener = testFilesAddRemoveListener;

            this.dte = serviceProvider.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;

            this.testFilesAddRemoveListener.TestFileChanged += OnProjectItemChanged;
            this.testFilesAddRemoveListener.StartListeningForTestFileChanges();

            this.solutionListener.SolutionUnloaded += SolutionListenerOnSolutionUnloaded;
            this.solutionListener.SolutionProjectChanged += OnSolutionProjectChanged;
            this.solutionListener.StartListeningForChanges();

            this.testFilesUpdateWatcher.FileChangedEvent += OnProjectItemChanged;
        }

        private void OnTestContainersChanged()
        {
            if (TestContainersUpdated != null && !initialContainerSearch)
            {
                TestContainersUpdated(this, EventArgs.Empty);
            }
        }

        private void SolutionListenerOnSolutionUnloaded(object sender, EventArgs eventArgs)
        {
            initialContainerSearch = true;
        }

        private void OnSolutionProjectChanged(object sender, SolutionEventsListenerEventArgs e)
        {
            if (e != null)
            {
                var files = GetTwinCATProjectFiles();

                if (e.ChangedReason == SolutionChangedReason.Load)
                {
                    UpdateFileWatcher(files, true);
                }
                else if (e.ChangedReason == SolutionChangedReason.Unload)
                {
                    UpdateFileWatcher(files, false);
                }
            }
        }

        private void UpdateFileWatcher(IEnumerable<string> files, bool isAdd)
        {
            foreach (var file in files)
            {
                if (isAdd)
                {
                    testFilesUpdateWatcher.AddWatch(file);
                    AddTestContainerIfTestFile(file);
                }
                else
                {
                    testFilesUpdateWatcher.RemoveWatch(file);
                    RemoveTestContainer(file);
                }
            }
        }

        private void OnProjectItemChanged(object sender, TestFileChangedEventArgs e)
        {
            if (e != null)
            {
                if (!IsFunctionBlock(e.File)) return;

                if(!IsTcUnitTestSuite(e.File)) return;

                switch (e.ChangedReason)
                {
                    case TestFileChangedReason.Added:
                        testFilesUpdateWatcher.AddWatch(e.TestContainer);
                        AddTestContainerIfTestFile(e.TestContainer);
                
                        break;
                    case TestFileChangedReason.Removed:
                        testFilesUpdateWatcher.RemoveWatch(e.TestContainer);
                        RemoveTestContainer(e.TestContainer);
                
                        break;
                    case TestFileChangedReason.Changed:
                        AddTestContainerIfTestFile(e.TestContainer);
                        break;
                }
            
                OnTestContainersChanged();
            }
        }

        private void AddTestContainerIfTestFile(string file)
        {
            var isTestFile = IsTcUnitTestContainer(file);
            RemoveTestContainer(file); 

            if (isTestFile)
            {
                var container = new TcUnitTestContainer(this, file, ExecutorUri);
                cachedContainers.Add(container);
            }
        }

        private void RemoveTestContainer(string file)
        {
            var index = cachedContainers.FindIndex(x => x.Source.Equals(file, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                cachedContainers.RemoveAt(index);
            }
        }

        private IEnumerable<ITestContainer> GetTestContainers()
        {
           if (initialContainerSearch)
            {
                cachedContainers.Clear();
                var twincatProjectFiles = GetTwinCATProjectFiles();
                UpdateFileWatcher(twincatProjectFiles, true);
                initialContainerSearch = false;
            }

            return cachedContainers;
        }

        private IEnumerable<string> GetTwinCATProjectFiles()
        {
			ThreadHelper.ThrowIfNotOnUIThread();

			var solution = (IVsSolution)serviceProvider.GetService(typeof(SVsSolution));
            var loadedProjects = solution.EnumerateLoadedProjects(__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION).OfType<IVsProject>();

            return loadedProjects.Select(VsSolutionExtensions.GetProjectPath)
                                    .Where(IsTwinCATProjectFile)
                                    .ToList();
        }

        private static bool IsTwinCATProjectFile(string path)
        {
            return ".tsproj".Equals(Path.GetExtension(path), StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsFunctionBlock(string path)
        {
            return ".TcPOU".Equals(Path.GetExtension(path), StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsTcUnitTestSuite(string path)
        {
            return XDocument.Load(path, LoadOptions.SetLineInfo).Element("TcPlcObject").Element("POU").Element("Declaration").Value.Contains("TcUnit.FB_TestSuite");
        }

        private bool IsTcUnitTestContainer(string path)
        {
            try
            {
                return IsTwinCATProjectFile(path);
            }
            catch (IOException e)
            {
                logger.Log(MessageLevel.Error, "IO error when detecting a test file during Test Container Discovery" + e.Message);
            }

            return false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (testFilesUpdateWatcher != null)
                {
                    testFilesUpdateWatcher.FileChangedEvent -= OnProjectItemChanged;
                    ((IDisposable)testFilesUpdateWatcher).Dispose();
                    testFilesUpdateWatcher = null;
                }

                if (testFilesAddRemoveListener != null)
                {
                    testFilesAddRemoveListener.TestFileChanged -= OnProjectItemChanged;
                    testFilesAddRemoveListener.StopListeningForTestFileChanges();
                    testFilesAddRemoveListener = null;
                }

                if (solutionListener != null)
                {
                    solutionListener.SolutionProjectChanged -= OnSolutionProjectChanged;
                    solutionListener.StopListeningForChanges();
                    solutionListener = null;
                }
            }
        }


    }



}
