using System;
using System.ComponentModel.Design;

using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TCatSysManagerLib;
using TcUnit_VsExtension.Dialogs;
using Task = System.Threading.Tasks.Task;

namespace TcUnit_VsExtension.Commands
{
    internal sealed class AddUnitTestSuiteCommand
    {
        public const int CommandId = PackageIds.AddUnitTestSuiteCommandId;
        public static readonly Guid CommandSet = PackageGuids.guidTcUnitPackageCmdSet;

        private readonly AsyncPackage package;
        private readonly TestSuiteFactory testSuiteFactory;

        private readonly EnvDTE.DTE dte;
        private AddUnitTestSuiteCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            menuItem.BeforeQueryStatus += new EventHandler(OnBeforeQueryStatus);
            commandService.AddCommand(menuItem);

            dte = Package.GetGlobalService(typeof(DTE)) as DTE;

            testSuiteFactory = new TestSuiteFactory();
        }

        public static AddUnitTestSuiteCommand Instance
        {
            get;
            private set;
        }

        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => package;

        private void OnBeforeQueryStatus(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var command = sender as OleMenuCommand;
            if (null != command)
            {
                command.Visible = false;
                ProjectItem selectedItem = dte?.SelectedItems?.Item(1)?.ProjectItem;
                command.Visible = CanAddTcUnitTestSuite(selectedItem);
            }
        }

        private bool CanAddTcUnitTestSuite (ProjectItem item)
        {
            if (item == null)
            {
                return false;
            }

            if (!(item?.Object is ITcSmTreeItem))
            {
                return false;
            }

            var treeItem = item.Object as ITcSmTreeItem;
            var isFolderOrPlcProject = treeItem.ItemType == (int)TCatSysManagerLib.TREEITEMTYPES.TREEITEMTYPE_PLCAPP
                || treeItem.ItemType == (int)TCatSysManagerLib.TREEITEMTYPES.TREEITEMTYPE_PLCFOLDER;

            return isFolderOrPlcProject;
        }

        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new AddUnitTestSuiteCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            ProjectItem selectedItem = dte.SelectedItems.Item(1).ProjectItem;

            Project plcProject = selectedItem.ContainingProject;
            ITcSmTreeItem plcProjectTreeItem = plcProject.Object as ITcSmTreeItem;

            if (selectedItem == null)
            {
                return;
            }

            if (!(selectedItem.Object is ITcSmTreeItem))
            {
                return;
            }

            ITcSmTreeItem treeItem = selectedItem.Object as ITcSmTreeItem;

            AddUnitTestSuiteDialogWindow dialog = new AddUnitTestSuiteDialogWindow();
            dialog.ShowModal();

            if (!dialog.DialogResult.HasValue || !dialog.DialogResult.Value)
            {
                return;
            }

            try
            {
                var testSuiteName = dialog.textboxName;
                testSuiteFactory.Create(testSuiteName, treeItem);
                InstantiateTestSuiteInCyclicProgram(plcProjectTreeItem, testSuiteName);

                dte.ExecuteCommand("File.SaveAll");

                NotificationProvider.DisplayInStatusBar($"Successfully added a new test suite \"{testSuiteName}\" to project \"{plcProjectTreeItem.Name}\"");
            }
            catch (Exception ex)
            {
                if(ex.HResult == -2147467259)
                {
                    NotificationProvider.DisplayInStatusBar($"Could not add new test suite! Test suite \"{dialog.textboxName}\" does already exist!");
                }
                else
                {
                    NotificationProvider.DisplayInStatusBar("Could not add new test suite!");
                }         
            }
        }

        private void InstantiateTestSuiteInCyclicProgram (ITcSmTreeItem plcProjectTreeItem, string testSuiteName)
        {
            var pouCall = FindTaskItem(plcProjectTreeItem)?.Name;
            var pouCalledByTask = FindPouTreeItemCalledByTask(plcProjectTreeItem, pouCall);

            if (pouCalledByTask is ITcPlcDeclaration)
            {
                ITcPlcDeclaration decl = pouCalledByTask as ITcPlcDeclaration;
                var declaration = decl.DeclarationText;

                var testSuiteInstance = string.Format($"\t{testSuiteName} : {testSuiteName};\r\nEND_VAR\r\n");
                declaration = declaration.Replace("END_VAR", testSuiteInstance);
                decl.DeclarationText = declaration;
            }
        }

        private ITcSmTreeItem FindTaskItem (ITcSmTreeItem plcTreeItem)
        {
            // start at bottom due to task is mostly placed at the bottom of plc project
            for (var i = plcTreeItem.ChildCount; i >= 1; i--)
            {
                var childItem = plcTreeItem.Child[i];
                if (childItem.ItemType == (int)TCatSysManagerLib.TREEITEMTYPES.TREEITEMTYPE_PLCTASK)
                {
                    return childItem.Child[1];
                }
            }

            return null;
        }

        private ITcSmTreeItem FindPouTreeItemCalledByTask (ITcSmTreeItem plcProjectItem, string pouName)
        {
            for (var i = 1; i <= plcProjectItem.ChildCount; i++)
            {
                var childItem = plcProjectItem.Child[i];

                if(childItem.Name == pouName
                    && childItem.ItemType == (int)TCatSysManagerLib.TREEITEMTYPES.TREEITEMTYPE_PLCPOUPROG)
                {
                    return childItem;
                }
                else if (childItem.ChildCount > 0
                        && childItem.ItemType == (int)TCatSysManagerLib.TREEITEMTYPES.TREEITEMTYPE_PLCFOLDER )
                {
                    var treeItem = FindPouTreeItemCalledByTask(childItem, pouName);

                    if(treeItem != null)
                    {
                        return treeItem;
                    }
                }
            }

            return null;
        }
    }
}
