using DevTimeMonitor.Entities;
using DevTimeMonitor.Views;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
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
        private static CommandEvents commandEvents;
        private static IVsTextManager textManager = Package.GetGlobalService(typeof(SVsTextManager)) as IVsTextManager;
        private static IVsTextView textView;

        private List<TbTracker> trackers;
        private TbUser user;
        private bool logged = false;
        private SettingsPage options;

        private static HashSet<string> FileTypes;

        private static int _beforeRow, _beforePosition, _afterRow, _afterPosition;

        private DevTimeMonitor(AsyncPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            FileTypes = new HashSet<string>()
            {
                "html",
                "css",
                "js",
                "ts",
                "c",
                "cpp",
                "cs",
                "java",
                "py",
                "sql",
                "xml",
                "json",
                "rb",
                "php",
                "swift",
                "go",
                "perl",
                "sh",
                "asm",
                "pl",
                "scss",
                "less",
                "sass",
                "jsx",
                "tsx",
                "xaml",
                "razor",
                "txt",
                "tt"
            };
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

                    Instance.options = await SettingsPage.GetLiveInstanceAsync();
                    Instance.logged = Instance.options.Logged;
                    if (Instance.logged)
                    {
                        using (ApplicationDBContext context = new ApplicationDBContext())
                        {
                            Instance.user = context.Users.Where(u => u.UserName == Instance.options.UserName).FirstOrDefault();
                        }
                    }

                    if (Instance.options.Autostart)
                    {
                        if (!Instance.logged)
                        {
                            OpenSettings(null, null);
                        }
                        else
                        {
                            TrackFiles(null, null);
                        }
                    }
                    ValidateConfiguration(package);
                }
            }
            catch (Exception ex)
            {
                if (Instance.logged)
                {
                    using (ApplicationDBContext context = new ApplicationDBContext())
                    {
                        TbError error = new TbError()
                        {
                            Detail = ex.Message,
                            UserId = Instance.user.Id
                        };
                        context.Errors.Add(error);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
        private static async void ValidateConfiguration(AsyncPackage package)
        {
            try
            {
                Instance.options = await SettingsPage.GetLiveInstanceAsync();
                do
                {
                    Instance.options = await SettingsPage.GetLiveInstanceAsync();
                    await Task.Delay(3000);
                } while (!Instance.options.Logged);

                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    Instance.user = context.Users.Where(u => u.UserName == Instance.options.UserName).FirstOrDefault();
                }
                Instance.logged = true;

                if (await package.GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
                {
                    MenuCommand trackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StartDevTimeMonitor));
                    trackFilesCommand.Enabled = true;
                    MenuCommand stopTrackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StopDevTimeMonitor));
                    stopTrackFilesCommand.Enabled = false;

                    if (Instance.options.Autostart)
                    {
                        trackFilesCommand.Enabled = false;
                        stopTrackFilesCommand.Enabled = true;
                    }

                    MenuCommand generateReportCommand = commandService.FindCommand(new CommandID(CommandSet, GenerateReport));
                    generateReportCommand.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                if (Instance.logged)
                {
                    using (ApplicationDBContext context = new ApplicationDBContext())
                    {
                        TbError error = new TbError()
                        {
                            Detail = ex.Message,
                            UserId = Instance.user.Id
                        };
                        context.Errors.Add(error);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }

        #region TRACKER
        private static async void TrackFiles(object sender, EventArgs e)
        {
            if (Instance.logged)
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
                                Document document = window.Document;
                                if (!document.ReadOnly && FileTypes.Contains(document.FullName.Split('.').Last()))
                                {
                                    Instance.OnWindowCreated(window);
                                }
                            }
                        }

                        dte.Events.WindowEvents.WindowCreated += Instance.OnWindowCreated;
                        dte.Events.WindowEvents.WindowClosing += Instance.OnWindowClosing;
                    }

                    outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                    customPane.Activate();
                    customPane.OutputStringThreadSafe("DevTimeMonitor Initialized");

                    DateTime currentTime = DateTime.Now.Date;
                    using (ApplicationDBContext context = new ApplicationDBContext())
                    {
                        Instance.trackers = context.Trackers.Where(t => t.UserId == Instance.user.Id && t.CreationDate >= currentTime).ToList() ?? new List<TbTracker>();
                        TbDailyLog dailyLog = context.DailyLogs.Where(d => d.UserId == Instance.user.Id).FirstOrDefault() ?? new TbDailyLog()
                        {
                            UserId = Instance.user.Id
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
                                dailyLog.Saturday = false;
                                dailyLog.Sunday = false;
                                break;
                            case DayOfWeek.Friday:
                                dailyLog.Friday = true;
                                dailyLog.Saturday = false;
                                dailyLog.Sunday = false;
                                break;
                            case DayOfWeek.Saturday:
                                dailyLog.Saturday = true;
                                dailyLog.Sunday = false;
                                break;
                            case DayOfWeek.Sunday:
                                dailyLog.Sunday = true;
                                break;
                            default:
                                break;
                        }

                        context.DailyLogs.AddOrUpdate(dailyLog);
                        await context.SaveChangesAsync();
                    }

                    customPane.OutputStringThreadSafe($"\nCurrent states saved: {Instance.trackers.Count}");

                    if (await Instance.package.GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
                    {
                        MenuCommand trackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StartDevTimeMonitor));
                        trackFilesCommand.Enabled = false;

                        MenuCommand stopTrackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StopDevTimeMonitor));
                        stopTrackFilesCommand.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    using (ApplicationDBContext context = new ApplicationDBContext())
                    {
                        TbError error = new TbError()
                        {
                            Detail = ex.Message,
                            UserId = Instance.user.Id
                        };
                        context.Errors.Add(error);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
        private static async void StopTrackingFiles(object sender, EventArgs e)
        {
            if (Instance.logged)
            {
                try
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(Instance.package.DisposalToken);

                    if (textEditorEvents != null)
                    {
                        textEditorEvents.LineChanged -= OnLineChanged;
                        textEditorEvents = null;
                    }
                    if (commandEvents != null)
                    {
                        commandEvents.AfterExecute -= CommandEvents_AfterExecute;
                        commandEvents.BeforeExecute -= CommandEvents_BeforeExecute;
                        commandEvents = null;
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
                        MenuCommand trackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StartDevTimeMonitor));
                        trackFilesCommand.Enabled = true;

                        MenuCommand stopTrackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StopDevTimeMonitor));
                        stopTrackFilesCommand.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    using (ApplicationDBContext context = new ApplicationDBContext())
                    {
                        TbError error = new TbError()
                        {
                            Detail = ex.Message,
                            UserId = Instance.user.Id
                        };
                        context.Errors.Add(error);
                        await context.SaveChangesAsync();
                    }
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
                if (Instance.logged)
                {
                    using (ApplicationDBContext context = new ApplicationDBContext())
                    {
                        TbError error = new TbError()
                        {
                            Detail = ex.Message,
                            UserId = user.Id
                        };
                        context.Errors.Add(error);
                        await context.SaveChangesAsync();
                    }
                }
                return "error";
            }
        }
        private async void OnWindowCreated(Window window)
        {
            if (Instance.logged)
            {
                try
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    if (window.Kind == "Document" && window.Document != null && window.Object != null && window.Document.Windows.Count <= 1)
                    {
                        Document document = window.Document;
                        if (!document.ReadOnly && FileTypes.Contains(document.FullName.Split('.').Last()))
                        {
                            if (textEditorEvents == null)
                            {
                                textEditorEvents = ((Events2)window.DTE.Events).TextEditorEvents;
                                textEditorEvents.LineChanged += OnLineChanged;
                            }
                            if (commandEvents == null)
                            {
                                commandEvents = ((Events2)window.DTE.Events).CommandEvents;
                                commandEvents.AfterExecute += CommandEvents_AfterExecute;
                                commandEvents.BeforeExecute += CommandEvents_BeforeExecute;
                            }

                            string filePath = document.FullName;
                            string projectName = window.Project.Name;
                            string fileName = filePath.Split('\\').Last();
                            string fileContent = await ReadFileContentAsync(filePath);
                            if (fileContent != "error")
                            {
                                outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                                customPane.OutputStringThreadSafe($"\nFile: {fileName} opened.");

                                DateTime currentTime = DateTime.Now.Date;
                                using (ApplicationDBContext context = new ApplicationDBContext())
                                {
                                    TbTracker tracker = context.Trackers.Where(t => t.UserId == user.Id && t.ProjectName == projectName && t.FileName == fileName && t.CreationDate >= currentTime).FirstOrDefault() ?? new TbTracker()
                                    {
                                        Id = Instance.trackers.Count,
                                        Path = filePath,
                                        ProjectName = projectName,
                                        FileName = fileName,
                                        CharactersTracked = 0,
                                        CharactersByCopilot = 0,
                                        UserId = user.Id,
                                        CreationDate = DateTime.Now
                                    };

                                    if (Instance.trackers.Find(t => t.UserId == user.Id && t.ProjectName == tracker.ProjectName && t.FileName == tracker.FileName && t.CreationDate >= currentTime) == null)
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
                        await context.SaveChangesAsync();
                    }
                }
            }
        }
        private async void OnWindowClosing(Window window)
        {
            if (Instance.logged)
            {
                try
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    if (window.Kind == "Document" && window.Object != null)
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
                                DateTime currentTime = DateTime.Now.Date;

                                using (ApplicationDBContext context = new ApplicationDBContext())
                                {
                                    if (context.Trackers.Where(t => t.UserId == user.Id && t.ProjectName == tracker.ProjectName && t.FileName == tracker.FileName && t.CreationDate >= currentTime).Any())
                                    {
                                        context.Entry(tracker).State = EntityState.Modified;
                                    }
                                    else
                                    {
                                        context.Trackers.Add(tracker);
                                    }

                                    await context.SaveChangesAsync();
                                }
                            }

                            outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                            customPane.OutputStringThreadSafe($"\nFile: {tracker.FileName} closed.");
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
                        await context.SaveChangesAsync();
                    }

                }
            }
        }

        private static void CommandEvents_BeforeExecute(string guid, int id, object customIn, object customOut, ref bool cancel)
        {
            if (id == (int)VSConstants.VSStd2KCmdID.TAB)
            {
                // Get the current cursor position
                textManager.GetActiveView(1, null, out textView);
                textView.GetCaretPos(out _beforeRow, out _beforePosition);
            }
        }
        private static async void CommandEvents_AfterExecute(string guid, int id, object customIn, object customOut)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (id == (int)VSConstants.VSStd2KCmdID.TAB)
            {
                // Get the current cursor position after the TAB
                textManager.GetActiveView(1, null, out textView);
                textView.GetCaretPos(out _afterRow, out _afterPosition);

                textView.GetTextStream(_beforeRow, _beforePosition, _afterRow, _afterPosition, out string text);
                int count = text.Count(c => !char.IsWhiteSpace(c));
                if (count > 0)
                {
                    if (await Instance.package.GetServiceAsync(typeof(DTE)) is DTE2 dte)
                    {
                        Window activeWindow = dte.ActiveWindow;
                        TbTracker tracker = Instance.trackers.Find(t =>
                        {
                            ThreadHelper.ThrowIfNotOnUIThread();
                            return t.FileName == activeWindow.Document.Name && t.Path == activeWindow.Document.FullName;
                        });
                        if (tracker != null)
                        {
                            tracker.CharactersByCopilot += count;
                            tracker.CharactersTracked += count;
                            outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                            customPane.OutputStringThreadSafe($"\nAccepted completion of length {count}");
                        }
                    }
                }
            }
        }

        private static async void OnLineChanged(TextPoint startPoint, TextPoint endPoint, int hint)
        {
            if (Instance.logged)
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

                                outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                                customPane.OutputStringThreadSafe($"\nCharacter entered by the user in {tracker.FileName}");
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
                            UserId = Instance.user.Id
                        };
                        context.Errors.Add(error);
                        await context.SaveChangesAsync();
                    }
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
            if (Instance.logged)
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