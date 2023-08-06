using System;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TCatSysManagerLib;
using TcUnit_VsExtension.Dialogs;
using Task = System.Threading.Tasks.Task;

namespace TcUnit_VsExtension.Commands
{
    internal sealed class AddUnitTestCaseCommand
    {
        public const int CommandId = PackageIds.AddUnitTestCaseCommandId;
        public static readonly Guid CommandSet = PackageGuids.guidTcUnitPackageCmdSet;

        private readonly AsyncPackage package;
        private readonly TestCaseFactory testCaseFactory;

        private readonly EnvDTE.DTE dte;

        private AddUnitTestCaseCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            menuItem.BeforeQueryStatus += new EventHandler(OnBeforeQueryStatus);
            commandService.AddCommand(menuItem);

            this.dte = Package.GetGlobalService(typeof(DTE)) as DTE;

            testCaseFactory = new TestCaseFactory();
        }

        public static AddUnitTestCaseCommand Instance
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
                ProjectItem selectedItem = dte.SelectedItems?.Item(1)?.ProjectItem;
                command.Visible = IsTcUnitTestSuite(selectedItem);
            }
        }

        private bool IsTcUnitTestSuite (ProjectItem item)
        {
            if (!(item?.Object is ITcSmTreeItem))
            {
                return false;
            }

            ITcSmTreeItem treeItem = item.Object as ITcSmTreeItem;
            var isFunctionBlock = treeItem.ItemType == (int)TCatSysManagerLib.TREEITEMTYPES.TREEITEMTYPE_PLCPOUFB;

            if (!isFunctionBlock)
            {
                return false;
            }

            ITcPlcDeclaration fbDecl = treeItem as ITcPlcDeclaration;
            var isTestSuite = fbDecl.DeclarationText.Contains("EXTENDS TcUnit.FB_TestSuite");

            return isFunctionBlock && isTestSuite;
        }


        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync((typeof(IMenuCommandService))) as OleMenuCommandService;
            Instance = new AddUnitTestCaseCommand(package, commandService);
        }

        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            ProjectItem selectedItem = dte.SelectedItems.Item(1).ProjectItem;
            
            if (!(selectedItem.Object is ITcSmTreeItem))
            {
                return;
            }

            ITcSmTreeItem treeItem = selectedItem.Object as ITcSmTreeItem;

            AddUnitTestCaseDialogWindow dialog = new AddUnitTestCaseDialogWindow();
            dialog.ShowModal();

            if (!dialog.DialogResult.HasValue || !dialog.DialogResult.Value)
            {
                return;
            }

            try
            {
                testCaseFactory.Create(dialog.textboxName, treeItem);
                selectedItem.Save("");

                dte.ExecuteCommand("File.SaveAll");

                NotificationProvider.DisplayInStatusBar($"Successfully added a new test case \"{dialog.textboxName}\" to test suite \"{treeItem.Name}\"");
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2147467259)
                {
                    NotificationProvider.DisplayInStatusBar($"Could not add new test case! Test case \"{dialog.textboxName}\" does already exist!");
                }
                else
                {
                    NotificationProvider.DisplayInStatusBar("Could not add new test case!");
                }         
            }
        }
    }
}
