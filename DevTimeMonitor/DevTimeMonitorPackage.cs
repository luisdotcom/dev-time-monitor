using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace DevTimeMonitor
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("DevTimeMonitor", "Extension DevTimeMonitor load asynchronously", "2.4")]
    [Guid(DevTimeMonitorPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideOptionPage(typeof(OptionsProvider.SettingsPageOptions), "DevTimeMonitor", "SettingsPage", 0, 0, true, SupportsProfiles = true)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExistsAndFullyLoaded_string, PackageAutoLoadFlags.BackgroundLoad)]
    public sealed class DevTimeMonitorPackage : AsyncPackage
    {
        public const string PackageGuidString = "71ce9d33-78f7-4374-a75b-abdaf8bf30cc";

        #region Package Members

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await Task.Delay(3000);
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await DevTimeMonitor.InitializeAsync(this);
        }

        #endregion
    }
}
