using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using TcUnit.Options;
using Task = System.Threading.Tasks.Task;

namespace TcUnit.VisualStudio
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasMultipleProjects_string, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionHasSingleProject_string, PackageAutoLoadFlags.BackgroundLoad)]
    [InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version, IconResourceID = 400)] 
    [Guid(PackageGuids.guidTcUnitPackageString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
	[ProvideOptionPage(typeof(GeneralOptionsPage), GeneralOptionsPage.Category, GeneralOptionsPage.Name, 0, 0, true)]
	[ProvideProfile(typeof(GeneralOptionsPage), GeneralOptionsPage.Category, GeneralOptionsPage.Name, 0, 0, true)]
	public sealed class TcUnitPackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
			await this.RegisterCommandsAsync();
		}
    }
}
