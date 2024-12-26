using System;
using System.ComponentModel.Design;
using System.Text.RegularExpressions;
using Community.VisualStudio.Toolkit;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TCatSysManagerLib;
using TcUnit.Options;
using TcUnit.VisualStudio.Dialogs;
using TcUnit.VisualStudio.Factories;
using TwinpackVsixShared.Extensions;
using Task = System.Threading.Tasks.Task;

namespace TcUnit.VisualStudio.Commands
{
	[Command(PackageGuids.guidTcUnitPackageCmdSetString, PackageIds.AddUnitTestCaseCommandId)]
	internal sealed class AddUnitTestCaseCommand : BaseCommand<AddUnitTestCaseCommand>
    {
        private readonly TestCaseFactory testCaseFactory;

        public AddUnitTestCaseCommand()
        {
            testCaseFactory = new TestCaseFactory();
		}

		protected override void BeforeQueryStatus(EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			Command.Visible = false;
			var dte = VS.GetRequiredService<DTE, DTE>();
			ProjectItem selectedItem = dte.SelectedItems?.Item(1)?.ProjectItem;
			Command.Visible = IsTcUnitTestSuite(selectedItem);
		}

        private bool IsTcUnitTestSuite (ProjectItem item)
        {
            if (!(item?.Object is ITcSmTreeItem treeItem))
				return false;

            if (!treeItem.IsPlcFunctionBlock())
				return false;

			ITcPlcDeclaration fbDecl = treeItem as ITcPlcDeclaration;
            var isTestSuite = fbDecl.DeclarationText.Contains("EXTENDS TcUnit.FB_TestSuite");

            return isTestSuite;
        }

		protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
		{
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

			var dte = await VS.GetRequiredServiceAsync<DTE, DTE>();
			ProjectItem selectedItem = dte.SelectedItems.Item(1).ProjectItem;
            
            if (!(selectedItem.Object is ITcSmTreeItem treeItem))
				return;

			AddUnitTestCaseDialog dialog = new AddUnitTestCaseDialog();
			dialog.ShowDialog();

            if (!dialog.DialogResult.HasValue || !dialog.DialogResult.Value)
				return;

			var testCaseName = dialog.textboxName;

            if (!Regex.IsMatch(testCaseName, General.Instance.TestCaseNamingRegex))
            {
				await VS.StatusBar.ShowMessageAsync("Could not add new test case! Invalid test case name!");
                return;
            }

            try
            {
                var testCaseTemplate = General.Instance.TestCaseTemplate; 

                testCaseFactory.Create(testCaseName, treeItem, testCaseTemplate);
                selectedItem.Save("");

				await VS.Commands.ExecuteAsync("File.SaveAll");

				await VS.StatusBar.ShowMessageAsync($"Successfully added a new test case \"{testCaseName}\" to test suite \"{treeItem.Name}\"");
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2147467259)
					await VS.StatusBar.ShowMessageAsync($"Could not add new test case! Test case \"{testCaseName}\" does already exist!");
				else
					await VS.StatusBar.ShowMessageAsync("Could not add new test case!");
			}
        }
    }
}
