using DevTimeMonitor.DTOs;
using DevTimeMonitor.Entities;
using DevTimeMonitor.Views;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Threading;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Composition;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTimeMonitor
{
    [Export]
    internal sealed class DevTimeMonitor
    {
        public const int StartDevTimeMonitor = 4129;
        public const int StopDevTimeMonitor = 4130;
        public const int GenerateReport = 4131;
        public const int Settings = 4132;

        public static readonly Guid CommandSet = new Guid("009c50db-7ae1-4460-acd1-da1112d471b0");
        private readonly AsyncPackage package;
        private static Guid outputGuid = new Guid("009c50db-7ae1-4460-acd1-da1112d471b1");
        private static readonly string outputTitle = "DevTimeMonitor";
        private static IVsOutputWindow outputWindow;
        private static TextEditorEvents textEditorEvents;

        private List<TbTracker> trackers;
        private static readonly SettingsHelper settingsHelper = new SettingsHelper();
        private static TbUser user;
        private static bool configured = false;
        private DevTimeMonitor(AsyncPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
        }
        public static DevTimeMonitor Instance { get; private set; }
        public static async Task InitializeAsync(AsyncPackage package)
        {
            try
            {
                Instance = new DevTimeMonitor(package);

                if (await package.GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
                {
                    CommandID trackFilesCommandID = new CommandID(CommandSet, StartDevTimeMonitor);
                    MenuCommand trackFilesSubItem = new MenuCommand(new EventHandler(TrackFiles), trackFilesCommandID);
                    commandService.AddCommand(trackFilesSubItem);

                    CommandID stopTrackFilesCommandID = new CommandID(CommandSet, StopDevTimeMonitor);
                    MenuCommand stopTrackFilesSubItem = new MenuCommand(new EventHandler(StopTrackingFiles), stopTrackFilesCommandID);
                    commandService.AddCommand(stopTrackFilesSubItem);

                    CommandID generateReportCommandID = new CommandID(CommandSet, GenerateReport);
                    MenuCommand generateReportSubItem = new MenuCommand(new EventHandler(ShowStatistics), generateReportCommandID);
                    commandService.AddCommand(generateReportSubItem);

                    CommandID openSettingsSubCommandID = new CommandID(CommandSet, Settings);
                    MenuCommand settingsSubItem = new MenuCommand(new EventHandler(OpenSettings), openSettingsSubCommandID);
                    commandService.AddCommand(settingsSubItem);

                    SettingsDTO settings = settingsHelper.ReadSettings();
                    if (!settings.Configured)
                    {
                        var trackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StartDevTimeMonitor));
                        if (trackFilesCommand != null)
                        {
                            trackFilesCommand.Enabled = false;
                        }

                        var stopTrackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StopDevTimeMonitor));
                        if (stopTrackFilesCommand != null)
                        {
                            stopTrackFilesCommand.Enabled = false;
                        }

                        var generateReportCommand = commandService.FindCommand(new CommandID(CommandSet, GenerateReport));
                        if (generateReportCommand != null)
                        {
                            generateReportCommand.Enabled = false;
                        }

                        OpenSettings(null, null);
                    }
                }

                ValidateConfiguration(package);
            }
            catch (Exception ex)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                string message = ex.Message;
                string title = "An error occurred";

                VsShellUtilities.ShowMessageBox(
                    null,
                    message,
                    title,
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }
        private static async void ValidateConfiguration(AsyncPackage package)
        {
            try
            {
                SettingsDTO settings;
                do
                {
                    settings = settingsHelper.ReadSettings();
                    await Task.Delay(3000);
                } while (!settings.Configured);

                if (await package.GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
                {
                    var trackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StartDevTimeMonitor));
                    if (trackFilesCommand != null)
                    {
                        trackFilesCommand.Enabled = true;
                    }

                    var stopTrackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StopDevTimeMonitor));
                    if (stopTrackFilesCommand != null)
                    {
                        stopTrackFilesCommand.Enabled = false;
                    }

                    var generateReportCommand = commandService.FindCommand(new CommandID(CommandSet, GenerateReport));
                    if (generateReportCommand != null)
                    {
                        generateReportCommand.Enabled = true;
                    }
                }
                using (var context = new ApplicationDBContext())
                {
                    user = context.Users.Where(u => u.UserName == settings.User).FirstOrDefault();
                }
                configured = true;
            }
            catch (Exception ex)
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                string message = ex.Message;
                string title = "An error occurred";

                VsShellUtilities.ShowMessageBox(
                    null,
                    message,
                    title,
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }

        #region TRACKER
        private static async void TrackFiles(object sender, EventArgs e)
        {
            if (configured)
            {
                try
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(Instance.package.DisposalToken);

                    outputWindow = (IVsOutputWindow)Package.GetGlobalService(typeof(SVsOutputWindow));
                    outputWindow.CreatePane(ref outputGuid, outputTitle, 1, 1);

                    if (await Instance.package.GetServiceAsync(typeof(DTE)) is DTE2 dte)
                    {
                        foreach (Window window in dte.Windows)
                        {
                            if (window.Kind == "Document" && window.Document != null)
                            {
                                Instance.OnWindowCreated(window);
                            }
                        }

                        dte.Events.WindowEvents.WindowCreated += Instance.OnWindowCreated;
                        dte.Events.WindowEvents.WindowClosing += Instance.OnWindowClosing;
                    }

                    outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                    customPane.Activate();
                    customPane.OutputStringThreadSafe("DevTimeMonitor Initialized");

                    using (var context = new ApplicationDBContext())
                    {
                        Instance.trackers = context.Trackers.Where(t => t.UserId == user.Id).ToList();
                        TbDailyLog dailyLog = context.DailyLogs.Where(d => d.UserId == user.Id).FirstOrDefault() ?? new TbDailyLog()
                        {
                            UserId = user.Id
                        };

                        DayOfWeek day = DateTime.Now.DayOfWeek;
                        switch (day)
                        {
                            case DayOfWeek.Monday:
                                dailyLog.Monday = true;

                                dailyLog.Tuesday = false;
                                dailyLog.Wednesday = false;
                                dailyLog.Thursday = false;
                                dailyLog.Friday = false;
                                break;
                            case DayOfWeek.Tuesday:
                                dailyLog.Tuesday = true;

                                dailyLog.Wednesday = false;
                                dailyLog.Thursday = false;
                                dailyLog.Friday = false;
                                break;
                            case DayOfWeek.Wednesday:
                                dailyLog.Wednesday = true;

                                dailyLog.Thursday = false;
                                dailyLog.Friday = false;
                                break;
                            case DayOfWeek.Thursday:
                                dailyLog.Thursday = true;
                                dailyLog.Friday = false;
                                break;
                            case DayOfWeek.Friday:
                                dailyLog.Friday = true;
                                break;
                            default:
                                break;
                        }

                        context.DailyLogs.AddOrUpdate(dailyLog);
                        context.SaveChanges();
                    }

                    customPane.OutputStringThreadSafe($"\nCurrent states saved: {Instance.trackers.Count}");

                    if (await Instance.package.GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
                    {
                        var trackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StartDevTimeMonitor));
                        if (trackFilesCommand != null)
                        {
                            trackFilesCommand.Enabled = false;
                        }

                        var stopTrackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StopDevTimeMonitor));
                        if (stopTrackFilesCommand != null)
                        {
                            stopTrackFilesCommand.Enabled = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (var context = new ApplicationDBContext())
                    {
                        TbError error = new TbError()
                        {
                            Detail = ex.Message,
                            UserId = user.Id
                        };
                        context.Errors.Add(error);
                        context.SaveChanges();
                    }

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    string message = ex.Message;
                    string title = "An error occurred";

                    VsShellUtilities.ShowMessageBox(
                        null,
                        message,
                        title,
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                }
            }
        }
        private static async void StopTrackingFiles(object sender, EventArgs e)
        {
            if (configured)
            {
                try
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(Instance.package.DisposalToken);

                    if (textEditorEvents != null)
                    {
                        textEditorEvents.LineChanged -= OnLineChanged;
                        textEditorEvents = null;
                    }

                    if (await Instance.package.GetServiceAsync(typeof(DTE)) is DTE2 dte)
                    {
                        dte.Events.WindowEvents.WindowCreated -= Instance.OnWindowCreated;
                        dte.Events.WindowEvents.WindowClosing -= Instance.OnWindowClosing;
                    }

                    outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                    customPane.OutputStringThreadSafe("\nDevTimeMonitor Finalized");

                    if (await Instance.package.GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
                    {
                        var trackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StartDevTimeMonitor));
                        if (trackFilesCommand != null)
                        {
                            trackFilesCommand.Enabled = true;
                        }

                        var stopTrackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StopDevTimeMonitor));
                        if (stopTrackFilesCommand != null)
                        {
                            stopTrackFilesCommand.Enabled = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (var context = new ApplicationDBContext())
                    {
                        TbError error = new TbError()
                        {
                            Detail = ex.Message,
                            UserId = user.Id
                        };
                        context.Errors.Add(error);
                        context.SaveChanges();
                    }

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    string message = ex.Message;
                    string title = "An error occurred";

                    VsShellUtilities.ShowMessageBox(
                        null,
                        message,
                        title,
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                }
            }
        }
        private async Task<string> ReadFileContentAsync(string filePath)
        {
            try
            {
                using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            catch (Exception ex)
            {
                if (configured)
                {
                    using (var context = new ApplicationDBContext())
                    {
                        TbError error = new TbError()
                        {
                            Detail = ex.Message,
                            UserId = user.Id
                        };
                        context.Errors.Add(error);
                        context.SaveChanges();
                    }
                }

                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                string message = ex.Message;
                string title = "An error occurred";

                VsShellUtilities.ShowMessageBox(
                    null,
                    message,
                    title,
                    OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);

                return "";
            }
        }
        private async void OnWindowCreated(Window window)
        {
            if (configured)
            {
                try
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    if (window.Kind == "Document" && window.Document != null)
                    {
                        Document document = window.Document;
                        if (!document.ReadOnly && !document.FullName.Contains(".csproj"))
                        {
                            if (textEditorEvents == null)
                            {
                                textEditorEvents = ((Events2)window.DTE.Events).TextEditorEvents;
                                textEditorEvents.LineChanged += OnLineChanged;
                            }
                            string filePath = document.FullName;
                            string projectName = window.Project.Name;
                            string fileName = filePath.Split('\\').Last();
                            string fileContent = await ReadFileContentAsync(filePath);

                            if (fileContent != "")
                            {
                                outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                                customPane.OutputStringThreadSafe($"\nFile: {fileName} opened.");

                                DateTime currentTime = DateTime.Now;
                                using (var context = new ApplicationDBContext())
                                {
                                    TbTracker tracker = context.Trackers.Where(t => t.UserId == user.Id && t.ProjectName == projectName && t.FileName == fileName).FirstOrDefault() ?? new TbTracker()
                                    {
                                        Id = Instance.trackers.Count,
                                        Path = filePath,
                                        ProjectName = projectName,
                                        FileName = fileName,
                                        CharactersTracked = 0,
                                        KeysPressed = 0,
                                        UserId = user.Id
                                    };

                                    if (Instance.trackers.Find(t => t.UserId == user.Id  && t.ProjectName == tracker.ProjectName && t.FileName == tracker.FileName) == null)
                                    {
                                        Instance.trackers.Add(tracker);
                                        context.Trackers.Add(tracker);
                                    }
                                    else
                                    {
                                        trackers.Remove(trackers.Find(t => t.Id == tracker.Id));
                                        trackers.Add(tracker);
                                    }
                                    await context.SaveChangesAsync();
                                    customPane.OutputStringThreadSafe($"\nTracking File: {fileName}.");
                                }
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    using (ApplicationDBContext context = new ApplicationDBContext())
                    {
                        TbError error = new TbError()
                        {
                            Detail = ex.Message,
                            UserId = user.Id
                        };
                        context.Errors.Add(error);
                        context.SaveChanges();
                    }

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    string message = ex.Message;
                    string title = "An error occurred";

                    VsShellUtilities.ShowMessageBox(
                        null,
                        message,
                        title,
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                }
            }
        }
        private async void OnWindowClosing(Window window)
        {
            if (configured)
            {
                try
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    if (window.Kind == "Document" && window.Document != null)
                    {
                        TbTracker tracker = Instance.trackers.Find(t =>
                        {
                            ThreadHelper.ThrowIfNotOnUIThread();
                            return t.FileName == window.Caption;
                        });
                        if (tracker != null)
                        {
                            string filePath = tracker.Path;
                            string fileContent = await ReadFileContentAsync(filePath);

                            if (fileContent != "")
                            {
                                int characters = fileContent.Length;
                                DateTime currentTime = DateTime.Now;

                                using (var context = new ApplicationDBContext())
                                {
                                    if (context.Trackers.Where(t => t.UserId == user.Id && t.ProjectName == tracker.ProjectName && t.FileName == tracker.FileName).Any())
                                    {
                                        context.Entry(tracker).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        context.Trackers.Add(tracker);
                                    }

                                    context.SaveChanges();
                                }

                                outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                                customPane.OutputStringThreadSafe($"\nFile: {tracker.FileName} closed.");
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    using (ApplicationDBContext context = new ApplicationDBContext())
                    {
                        TbError error = new TbError()
                        {
                            Detail = ex.Message,
                            UserId = user.Id
                        };
                        context.Errors.Add(error);
                        context.SaveChanges();
                    }

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    string message = ex.Message;
                    string title = "An error occurred";

                    VsShellUtilities.ShowMessageBox(
                        null,
                        message,
                        title,
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                }
            }
        }
        private static async void OnLineChanged(TextPoint startPoint, TextPoint endPoint, int hint)
        {
            if (configured)
            {
                try
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    if (startPoint.Parent != null & startPoint.Parent.Parent != null)
                    {
                        TbTracker tracker = Instance.trackers.Find(t =>
                        {
                            ThreadHelper.ThrowIfNotOnUIThread();
                            return t.FileName == startPoint.Parent.Parent.ActiveWindow.Document.Name && t.Path == startPoint.Parent.Parent.ActiveWindow.Document.FullName;
                        });
                        if (tracker != null)
                        {
                            string modifiedText = startPoint.CreateEditPoint().GetText(endPoint);
                            modifiedText = modifiedText.Trim();
                            if ((modifiedText.Length == 1 || modifiedText == "\r\n" || modifiedText == "\r" || modifiedText == "\n") && IsAValidCharacter(modifiedText))
                            {
                                tracker.CharactersTracked++;
                                tracker.KeysPressed++;

                                outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                                customPane.OutputStringThreadSafe($"\nCharacter entered by the user in {tracker.FileName}");
                            }
                            else
                            {
                                tracker.CharactersTracked += modifiedText.Length;
                            }
                        }
                    }
                }
                catch (IOException ex)
                {
                    using (ApplicationDBContext context = new ApplicationDBContext())
                    {
                        TbError error = new TbError()
                        {
                            Detail = ex.Message,
                            UserId = user.Id
                        };
                        context.Errors.Add(error);
                        context.SaveChanges();
                    }

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    string message = ex.Message;
                    string title = "An error occurred";

                    VsShellUtilities.ShowMessageBox(
                        null,
                        message,
                        title,
                        OLEMSGICON.OLEMSGICON_CRITICAL,
                        OLEMSGBUTTON.OLEMSGBUTTON_OK,
                        OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
                }
            }
        }
        private static bool IsAValidCharacter(string text)
        {
            string characters = "!\"#$%&/()=?¡'¿+*{}[]-_:.;,<> ";
            return characters.Contains(text) || text.Any(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) || char.IsControl(c) || char.IsSymbol(c));
        }
        #endregion

        #region REPORTER
        private static void ShowStatistics(object sender, EventArgs e)
        {
            if (configured)
            {
                Report report = new Report();
                report.Show();
            }
        }
        #endregion

        #region Settings
        public static async void OpenSettings(object sender, EventArgs e)
        {
            Settings settings = new Settings();
            settings.Show();
        }
        #endregion
    }
}