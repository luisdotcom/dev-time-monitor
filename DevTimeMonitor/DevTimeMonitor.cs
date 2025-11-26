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
        private static DocumentEvents documentEvents;
        private static IVsTextManager textManager = Package.GetGlobalService(typeof(SVsTextManager)) as IVsTextManager;
        private static IVsTextView textView;

        private List<TbTracker> trackers;
        private TbUser user;
        private bool logged = false;
        private SettingsPage options;
        private bool isCommandExecution = false;
        private HashSet<string> currentlyProcessingDocumentPaths = new HashSet<string>();
        private readonly object processingLock = new object();

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
                "tt",
                "cshtml"
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

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    outputWindow = (IVsOutputWindow)Package.GetGlobalService(typeof(SVsOutputWindow));
                    outputWindow.CreatePane(ref outputGuid, outputTitle, 1, 1);

                    await ValidateConfigurationAsync(package);
                }
            }
            catch (Exception ex)
            {
                await PrintMessageAsync(ex.Message);
            }
        }
        private static async Task ValidateConfigurationAsync(AsyncPackage package)
        {
            try
            {
                Instance.options = await SettingsPage.GetLiveInstanceAsync();
                string message = "";

                if (await VerifyDatabaseConnectionAsync())
                {
                    Instance.logged = false;

                    if (Instance.options.UserName != "")
                    {
                        using (ApplicationDBContext context = new ApplicationDBContext())
                        {
                            Instance.user = context.Users.Where(u => u.UserName == Instance.options.UserName).FirstOrDefault();
                            if (Instance.user == null)
                            {
                                message = "No existe el usuario en la base de datos";
                            }
                            else
                            {
                                Instance.logged = true;
                            }
                        }
                    }
                    else
                    {
                        message = "Inicia sesión para utilizar la extensión";
                    }

                    if (await package.GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
                    {
                        MenuCommand trackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StartDevTimeMonitor));
                        MenuCommand stopTrackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, StopDevTimeMonitor));
                        MenuCommand generateReportCommand = commandService.FindCommand(new CommandID(CommandSet, GenerateReport));

                        if (Instance.logged)
                        {
                            trackFilesCommand.Enabled = !Instance.options.Autostart;
                            stopTrackFilesCommand.Enabled = Instance.options.Autostart;
                            generateReportCommand.Enabled = true;

                            if (Instance.options.Autostart)
                            {
                                TrackFiles(null, null);
                            }
                        }
                        else
                        {
                            trackFilesCommand.Enabled = false;
                            stopTrackFilesCommand.Enabled = false;
                            generateReportCommand.Enabled = false;

                            Instance.options.Logged = false;
                            Instance.options.UserName = "";
                            Instance.options.Password = "";
                            Instance.options.Name = "";

                            Instance.options.Autostart = false;
                            await Instance.options.SaveAsync();

                            Instance.options = await SettingsPage.GetLiveInstanceAsync();
                        }
                    }
                }
                else
                {
                    message = "No se puede conectar a la base de datos";
                }

                if (message != "")
                {
                    await PrintMessageAsync(message);
                }
            }
            catch (Exception ex)
            {
                await PrintMessageAsync(ex.Message);
            }
        }
        private static async Task<bool> VerifyDatabaseConnectionAsync()
        {
            try
            {
                using (ApplicationDBContext context = new ApplicationDBContext())
                {
                    await context.Database.Connection.OpenAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                await PrintMessageAsync(ex.Message);
                return false;
            }
        }
        private static async Task PrintMessageAsync(string message)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(Instance.package.DisposalToken);

            outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
            customPane.Activate();
            customPane.OutputStringThreadSafe("\n" + message);
        }

        #region TRACKER
        private static async void TrackFiles(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            if (Instance.logged)
            {
                try
                {
                    if (await Instance.package.GetServiceAsync(typeof(DTE)) is DTE2 dte)
                    {
                        Instance.EnsureEventsSubscribed(dte);

                        foreach (Document document in dte.Documents)
                        {
                            if (document != null && !document.ReadOnly)
                            {
                                string fileExtension = Path.GetExtension(document.FullName)?.TrimStart('.')?.ToLower();
                                if (!string.IsNullOrEmpty(fileExtension) && FileTypes.Contains(fileExtension))
                                {
                                    await Instance.ProcessDocumentAsync(document);
                                }
                            }
                        }
                    }

                    await PrintMessageAsync("DevTimeMonitor Initialized");

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
                                dailyLog.Saturday = false;
                                dailyLog.Sunday = false;
                                break;
                            case DayOfWeek.Tuesday:
                                dailyLog.Tuesday = true;

                                dailyLog.Wednesday = false;
                                dailyLog.Thursday = false;
                                dailyLog.Friday = false;
                                dailyLog.Saturday = false;
                                dailyLog.Sunday = false;
                                break;
                            case DayOfWeek.Wednesday:
                                dailyLog.Wednesday = true;

                                dailyLog.Thursday = false;
                                dailyLog.Friday = false;
                                dailyLog.Saturday = false;
                                dailyLog.Sunday = false;
                                break;
                            case DayOfWeek.Thursday:
                                dailyLog.Thursday = true;

                                dailyLog.Friday = false;
                                dailyLog.Saturday = false;
                                dailyLog.Sunday = false;
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

                    await PrintMessageAsync($"Current states saved: {Instance.trackers.Count}");

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
                    await PrintMessageAsync(ex.Message);
                    StopTrackingFiles(null, null);
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
                    if (documentEvents != null)
                    {
                        documentEvents.DocumentOpened -= Instance.DocumentOpenedHandler;
                        documentEvents.DocumentSaved -= Instance.DocumentSavedHandler;
                        documentEvents = null;
                    }

                    await PrintMessageAsync("DevTimeMonitor Stopped");

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
                    await PrintMessageAsync(ex.Message);
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
                await PrintMessageAsync(ex.Message);
                StopTrackingFiles(null, null);
                return "error";
            }
        }
        private async Task ProcessDocumentAsync(Document document)
        {
            if (!Instance.logged) return;

            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (document == null || document.ReadOnly) return;

                string fileExtension = Path.GetExtension(document.FullName)?.TrimStart('.')?.ToLower();
                if (string.IsNullOrEmpty(fileExtension) || !FileTypes.Contains(fileExtension)) return;

                string filePath = document.FullName;
                string fileName = Path.GetFileName(filePath);
                string projectName = document.ProjectItem?.ContainingProject?.Name ?? "Unknown";

                await FindOrCreateTrackerAsync(projectName, filePath, fileName);
            }
            catch (Exception ex)
            {
                await PrintMessageAsync($"Error processing document: {ex.Message}");
            }
        }

        private async void DocumentOpenedHandler(Document document)
        {
            if (!Instance.logged) return;

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(Instance.package.DisposalToken);

            string filePath = document.FullName;

            bool shouldProcess = false;
            lock (Instance.processingLock)
            {
                if (!Instance.currentlyProcessingDocumentPaths.Contains(filePath))
                {
                    Instance.currentlyProcessingDocumentPaths.Add(filePath);
                    shouldProcess = true;
                }
            }

            if (!shouldProcess) return;

            try
            {
                string fileExtension = Path.GetExtension(filePath)?.TrimStart('.')?.ToLower();
                if (!Instance.trackers.Any(t => t.Path == filePath) &&
                    !string.IsNullOrEmpty(fileExtension) &&
                    FileTypes.Contains(fileExtension))
                {
                    await Instance.ProcessDocumentAsync(document);
                }
            }
            finally
            {
                lock (Instance.processingLock)
                {
                    Instance.currentlyProcessingDocumentPaths.Remove(filePath);
                }
            }
        }
        private async void DocumentSavedHandler(Document document)
        {
            if (!Instance.logged) return;

            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(Instance.package.DisposalToken);

                string filePath = document.FullName;
                string fileExtension = Path.GetExtension(filePath)?.TrimStart('.')?.ToLower();

                if (string.IsNullOrEmpty(fileExtension) || !FileTypes.Contains(fileExtension)) return;

                TbTracker tracker = Instance.trackers.FirstOrDefault(t => t.Path == filePath);

                if (tracker != null)
                {
                    string fileContent = await Instance.ReadFileContentAsync(filePath);

                    if (!string.IsNullOrEmpty(fileContent) && fileContent != "error")
                    {
                        using (ApplicationDBContext context = new ApplicationDBContext())
                        {
                            DateTime currentTime = DateTime.Now.Date;
                            if (context.Trackers.Any(t => t.UserId == Instance.user.Id &&
                                                          t.ProjectName == tracker.ProjectName &&
                                                          t.FileName == tracker.FileName &&
                                                          t.CreationDate >= currentTime))
                            {
                                context.Trackers.AddOrUpdate(tracker);
                            }
                            else
                            {
                                context.Trackers.Add(tracker);
                            }
                            await context.SaveChangesAsync();
                        }

                        await PrintMessageAsync($"Document saved: {tracker.FileName}");
                    }
                }
            }
            catch (Exception ex)
            {
                await PrintMessageAsync($"Error saving document: {ex.Message}");
            }
        }

        private async Task<bool> FindOrCreateTrackerAsync(string projectName, string filePath, string fileName)
        {
            try
            {
                string fileContent = await ReadFileContentAsync(filePath);
                if (fileContent == "error") return false;

                DateTime currentTime = DateTime.Now.Date;
                TbTracker tracker = trackers.FirstOrDefault(t =>
                    t.UserId == user.Id &&
                    t.ProjectName == projectName &&
                    t.FileName == fileName &&
                    t.CreationDate >= currentTime);

                if (tracker == null)
                {
                    tracker = new TbTracker()
                    {
                        UserId = user.Id,
                        ProjectName = projectName,
                        FileName = fileName,
                        Path = filePath,
                        CharactersTracked = 0,
                        CharactersByCopilot = 0,
                        CreationDate = DateTime.Now,
                    };

                    using (ApplicationDBContext context = new ApplicationDBContext())
                    {
                        TbTracker existingTracker = context.Trackers.FirstOrDefault(t =>
                            t.UserId == user.Id &&
                            t.ProjectName == projectName &&
                            t.FileName == fileName &&
                            t.CreationDate >= currentTime);

                        if (existingTracker == null)
                        {
                            context.Trackers.Add(tracker);
                            await context.SaveChangesAsync();
                            trackers.Add(tracker);
                            await PrintMessageAsync($"New tracker created: {fileName}");
                        }
                        else
                        {
                            trackers.Add(existingTracker);
                            await PrintMessageAsync($"Existing tracker loaded: {fileName}");
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                await PrintMessageAsync($"Error finding/creating tracker: {ex.Message}");
                return false;
            }
        }

        private static void CommandEvents_BeforeExecute(string guid, int id, object customIn, object customOut, ref bool cancel)
        {
            if (!Instance.logged) return;

            try
            {
                Instance.isCommandExecution = true;

                if (id == (int)VSConstants.VSStd2KCmdID.TAB)
                {
                    textManager?.GetActiveView(1, null, out textView);
                    textView?.GetCaretPos(out _beforeRow, out _beforePosition);
                }
            }
            catch (Exception ex)
            {
                PrintMessageAsync($"Error in BeforeExecute: {ex.Message}").FireAndForget();
            }
        }
        private static async void CommandEvents_AfterExecute(string guid, int id, object customIn, object customOut)
        {
            if (!Instance.logged) return;

            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (id == (int)VSConstants.VSStd2KCmdID.TAB)
                {
                    textManager?.GetActiveView(1, null, out textView);
                    if (textView == null) return;

                    textView.GetCaretPos(out _afterRow, out _afterPosition);
                    textView.GetTextStream(_beforeRow, _beforePosition, _afterRow, _afterPosition, out string text);

                    int count = text?.Count(c => !char.IsWhiteSpace(c)) ?? 0;

                    if (count > 0)
                    {
                        if (await Instance.package.GetServiceAsync(typeof(DTE)) is DTE2 dte)
                        {
                            Document activeDocument = dte.ActiveDocument;
                            if (activeDocument != null)
                            {
                                string filePath = activeDocument.FullName;
                                TbTracker tracker = Instance.trackers.FirstOrDefault(t => t.Path == filePath);

                                if (tracker != null)
                                {
                                    tracker.CharactersByCopilot += count;
                                    tracker.CharactersTracked += count;
                                    await PrintMessageAsync($"Copilot completion: {count} characters");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await PrintMessageAsync($"Error in AfterExecute: {ex.Message}");
            }
            finally
            {
                Instance.isCommandExecution = false;
            }
        }

        private static async void OnLineChanged(TextPoint startPoint, TextPoint endPoint, int hint)
        {
            if (!Instance.logged || Instance.isCommandExecution) return;

            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                if (startPoint?.Parent?.Parent?.ActiveWindow?.Document == null) return;

                Document activeDocument = startPoint.Parent.Parent.ActiveWindow.Document;
                string filePath = activeDocument.FullName;
                string modifiedText = startPoint.CreateEditPoint().GetText(endPoint);

                if (string.IsNullOrEmpty(modifiedText)) return;

                TbTracker tracker = Instance.trackers.FirstOrDefault(t => t.Path == filePath);

                if (tracker != null)
                {
                    if ((modifiedText.Length == 1 || modifiedText == "\r\n" || modifiedText == "\r" || modifiedText == "\n") &&
                        IsValidCharacter(modifiedText))
                    {
                        tracker.CharactersTracked++;
                    }
                }
            }
            catch (Exception ex)
            {
                await PrintMessageAsync($"Error in OnLineChanged: {ex.Message}");
            }
        }
        private static bool IsValidCharacter(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;

            if (text == "\r\n" || text == "\r" || text == "\n") return true;

            return !char.IsWhiteSpace(text[0]);
        }
        #endregion

        private void EnsureEventsSubscribed(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (textEditorEvents == null)
            {
                Events2 events = (Events2)dte.Events;
                textEditorEvents = events.TextEditorEvents;
                if (textEditorEvents != null)
                {
                    textEditorEvents.LineChanged += OnLineChanged;
                }
                else
                {
                    PrintMessageAsync("Failed to get TextEditorEvents.").FireAndForget();
                }
            }

            if (commandEvents == null)
            {
                Events2 events = (Events2)dte.Events;
                commandEvents = events.CommandEvents;
                if (commandEvents != null)
                {
                    commandEvents.BeforeExecute += CommandEvents_BeforeExecute;
                    commandEvents.AfterExecute += CommandEvents_AfterExecute;
                }
                else
                {
                    PrintMessageAsync("Failed to get CommandEvents.").FireAndForget();
                }
            }

            if (documentEvents == null)
            {
                Events2 events = (Events2)dte.Events;
                documentEvents = events.DocumentEvents;
                if (documentEvents != null)
                {
                    documentEvents.DocumentOpened += DocumentOpenedHandler;
                    documentEvents.DocumentSaved += DocumentSavedHandler;
                }
                else
                {
                    PrintMessageAsync("Failed to get DocumentEvents.").FireAndForget();
                }
            }
        }

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