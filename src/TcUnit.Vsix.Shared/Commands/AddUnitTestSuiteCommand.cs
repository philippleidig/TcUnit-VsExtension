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
	[Command(PackageGuids.guidTcUnitPackageCmdSetString, PackageIds.AddUnitTestSuiteCommandId)]
	internal sealed class AddUnitTestSuiteCommand : BaseCommand<AddUnitTestSuiteCommand>
    {
        private readonly TestSuiteFactory testSuiteFactory;

        public AddUnitTestSuiteCommand()
        {
            testSuiteFactory = new TestSuiteFactory();
		}

		protected override void BeforeQueryStatus(EventArgs e)
		{
			ThreadHelper.ThrowIfNotOnUIThread();

			Command.Visible = false;
			var dte = VS.GetRequiredService<DTE, DTE>();
			ProjectItem selectedItem = dte?.SelectedItems?.Item(1)?.ProjectItem;
			Command.Visible = CanAddTcUnitTestSuite(selectedItem);
		}

        private bool CanAddTcUnitTestSuite (ProjectItem item)
        {
			ThreadHelper.ThrowIfNotOnUIThread();

			if (item == null)
				return false;

			if (!item.IsTwinCATTreeItem())
				return false;

			var treeItem = item.Object as ITcSmTreeItem;
            return treeItem.IsPlcProject() || treeItem.IsPlcProjectFolder();
        }

		protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
			await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

			var dte = await VS.GetRequiredServiceAsync<DTE, DTE>();
			ProjectItem selectedItem = dte.SelectedItems.Item(1).ProjectItem;

            EnvDTE.Project plcProject = selectedItem.ContainingProject;
            ITcSmTreeItem plcProjectTreeItem = plcProject.Object as ITcSmTreeItem;

            if (selectedItem == null)
				return;

			if (!(selectedItem.Object is ITcSmTreeItem treeItem))
				return;

            AddUnitTestSuiteDialog dialog = new AddUnitTestSuiteDialog();

			if((bool)!dialog.ShowDialog())
				return;

			var testSuiteName = dialog.textboxName;

            if (!Regex.IsMatch(testSuiteName, General.Instance.TestSuiteNamingRegex))
            {
				await VS.StatusBar.ShowMessageAsync("Could not add new test suite! Invalid test suite name!");
                return;
            }

            try
            {
                testSuiteFactory.Create(testSuiteName, treeItem);
                InstantiateTestSuiteInCyclicProgram(plcProjectTreeItem, testSuiteName);

				await VS.Commands.ExecuteAsync("File.SaveAll");
				await VS.StatusBar.ShowMessageAsync($"Successfully added a new test suite \"{testSuiteName}\" to project \"{plcProjectTreeItem.Name}\"");
            }
            catch (Exception ex)
            {
                if(ex.HResult == -2147467259)
					await VS.StatusBar.ShowMessageAsync($"Could not add new test suite! Test suite \"{testSuiteName}\" does already exist!");
				else
					await VS.StatusBar.ShowMessageAsync("Could not add new test suite!");
			}
        }

        private void InstantiateTestSuiteInCyclicProgram (ITcSmTreeItem plcProjectTreeItem, string testSuiteName)
        {
            var pouCall = FindTaskItem(plcProjectTreeItem)?.Name;
            var pouCalledByTask = FindPouTreeItemCalledByTask(plcProjectTreeItem, pouCall);

            if (pouCalledByTask is ITcPlcDeclaration declarationItem)
            {
                var declaration = declarationItem.DeclarationText;

                var testSuiteInstance = string.Format($"\t{testSuiteName} : {testSuiteName};\r\nEND_VAR\r\n");
                declaration = declaration.Replace("END_VAR", testSuiteInstance);
				declarationItem.DeclarationText = declaration;
            }
        }

        private ITcSmTreeItem FindTaskItem (ITcSmTreeItem plcTreeItem)
        {
            // start at bottom due to task is mostly placed at the bottom of plc project
            for (var i = plcTreeItem.ChildCount; i >= 1; i--)
            {
                var childItem = plcTreeItem.Child[i];

                if (childItem.IsPlcTask())
					return childItem.Child[1];
			}

            return null;
        }

        private ITcSmTreeItem FindPouTreeItemCalledByTask (ITcSmTreeItem plcProjectItem, string pouName)
        {
            for (var i = 1; i <= plcProjectItem.ChildCount; i++)
            {
                var childItem = plcProjectItem.Child[i];

                if(childItem.Name == pouName && childItem.IsPlcFunctionBlock())
					return childItem;
				else if (childItem.ChildCount > 0 && childItem.IsPlcProjectFolder() )
					return FindPouTreeItemCalledByTask(childItem, pouName);
			}

            return null;
        }
    }
}
