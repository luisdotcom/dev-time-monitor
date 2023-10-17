using Microsoft.VisualStudio.Shell;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace DevTimeMonitor
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(DevTimeMonitorPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class DevTimeMonitorPackage : AsyncPackage
    {
        public const string PackageGuidString = "71ce9d33-78f7-4374-a75b-abdaf8bf30cc";

        #region Package Members

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await DevTimeMonitor.InitializeAsync(this);
        }

        #endregion
    }
}
