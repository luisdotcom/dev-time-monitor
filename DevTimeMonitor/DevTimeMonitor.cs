using DevTimeMonitor.Data;
using DevTimeMonitor.Entities;
using DevTimeMonitor.Views;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Composition;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevTimeMonitor
{
    [Export]
    internal sealed class DevTimeMonitor
    {
        public const int CommandId = 4129;
        public const int cmdidStopDevTimeMonitor = 4130;
        public const int GenerateReportCommandId = 4433;

        public static readonly Guid CommandSet = new Guid("009c50db-7ae1-4460-acd1-da1112d471b0");
        private readonly AsyncPackage package;
        private static DataManager dataManager;
        private List<Tracker> trackers;
        private static Guid outputGuid = new Guid("009c50db-7ae1-4460-acd1-da1112d471b1");
        private static readonly string outputTitle = "DevTimeMonitor";
        private static IVsOutputWindow outputWindow;
        private static TextEditorEvents textEditorEvents;
        private static string previousKeyPressed = "";
        private static string user = "";
        private DevTimeMonitor(AsyncPackage package)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            dataManager = new DataManager();
        }

        public static DevTimeMonitor Instance { get; private set; }
        public static async Task InitializeAsync(AsyncPackage package)
        {
            Instance = new DevTimeMonitor(package);
            user = Environment.UserName;
            if (await package.GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                CommandID trackFilesCommandID = new CommandID(CommandSet, CommandId);
                MenuCommand trackFilesSubItem = new MenuCommand(new EventHandler(TrackFiles), trackFilesCommandID);
                commandService.AddCommand(trackFilesSubItem);

                CommandID stopTrackFilesCommandID = new CommandID(CommandSet, cmdidStopDevTimeMonitor);
                MenuCommand stopTrackFilesSubItem = new MenuCommand(new EventHandler(StopTrackingFiles), stopTrackFilesCommandID);
                commandService.AddCommand(stopTrackFilesSubItem);

                CommandID generateReportSubCommandID = new CommandID(CommandSet, GenerateReportCommandId);
                MenuCommand generateReportSubItem = new MenuCommand(new EventHandler(ShowStatistics), generateReportSubCommandID);
                commandService.AddCommand(generateReportSubItem);
            }
        }

        #region TRACKER
        private static async void TrackFiles(object sender, EventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(Instance.package.DisposalToken);

            outputWindow = (IVsOutputWindow)Package.GetGlobalService(typeof(SVsOutputWindow));
            outputWindow.CreatePane(ref outputGuid, outputTitle, 1, 1);

            if (await Instance.package.GetServiceAsync(typeof(DTE)) is DTE2 dte)
            {
                foreach (Window window in dte.Windows)
                {
                    if (window.Kind == "Document")
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
            Instance.trackers = dataManager.ReadAll();
            customPane.OutputStringThreadSafe($"\nCurrent states saved: {Instance.trackers.Count}");

            if (await Instance.package.GetServiceAsync(typeof(IMenuCommandService)) is OleMenuCommandService commandService)
            {
                var trackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, CommandId));
                if (trackFilesCommand != null)
                {
                    trackFilesCommand.Enabled = false;
                }

                var stopTrackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, cmdidStopDevTimeMonitor));
                if (stopTrackFilesCommand != null)
                {
                    stopTrackFilesCommand.Enabled = true;
                }
            }
        }
        private static async void StopTrackingFiles(object sender, EventArgs e)
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
                var trackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, CommandId));
                if (trackFilesCommand != null)
                {
                    trackFilesCommand.Enabled = true;
                }

                var stopTrackFilesCommand = commandService.FindCommand(new CommandID(CommandSet, cmdidStopDevTimeMonitor));
                if (stopTrackFilesCommand != null)
                {
                    stopTrackFilesCommand.Enabled = false;
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
            catch { return null; }
        }
        private async void OnWindowCreated(Window window)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            try
            {
                if (window.Kind == "Document")
                {
                    Document document = window.Document;
                    if (!document.ReadOnly)
                    {
                        if(textEditorEvents == null)
                        {
                            textEditorEvents = ((Events2)window.DTE.Events).TextEditorEvents;
                            textEditorEvents.LineChanged += OnLineChanged;
                        }
                        string filePath = document.FullName;
                        string projectName = window.Project.Name;
                        string fileName = filePath.Split('\\').Last();
                        string fileContent = await ReadFileContentAsync(filePath);

                        if (fileContent != null)
                        {
                            outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                            customPane.OutputStringThreadSafe($"\nFile: {fileName} opened.");

                            int characters = fileContent.Length;
                            DateTime currentTime = DateTime.Now;

                            Tracker tracker = dataManager.Search(projectName, fileName) ?? new Tracker()
                            {
                                Id = Instance.trackers.Count,
                                Path = filePath,
                                ProjectName = projectName,
                                FileName = fileName,
                                PreviousCharacters = characters,
                                StartTime = currentTime
                            };

                            tracker.PreviousCharacters = characters;
                            tracker.StartTime = currentTime;
                            tracker.ClosingTime = currentTime;
                            tracker.NewCharacters = 0;
                            tracker.NewKeysPressed = 0;

                            if (Instance.trackers.Find(t => t.ProjectName == tracker.ProjectName && t.FileName == tracker.FileName) == null)
                            {
                                Instance.trackers.Add(tracker);
                                dataManager.Insert(tracker);
                            }
                            else
                            {
                                trackers.Remove(trackers.Find(t => t.Id == tracker.Id));

                                tracker.NewCharacters = 0;
                                tracker.NewKeysPressed = 0;

                                trackers.Add(tracker);

                                dataManager.Update(tracker);
                            }

                            customPane.OutputStringThreadSafe($"\nTracking File: {fileName}.");
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                customPane.OutputStringThreadSafe($"\nAn error has occurred: {ex.Message}");
            }

        }
        private async void OnWindowClosing(Window window)
        {
            try
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                if (window.Kind == "Document")
                {
                    Tracker tracker = Instance.trackers.Find(t =>
                    {
                        ThreadHelper.ThrowIfNotOnUIThread();
                        return t.FileName == window.Caption;
                    });
                    if (tracker != null)
                    {
                        string filePath = tracker.Path;
                        string fileContent = await ReadFileContentAsync(filePath);

                        if (fileContent != null)
                        {
                            int characters = fileContent.Length;
                            if (tracker.TotalCharacters != characters || tracker.NewKeysPressed != 0)
                            {
                                DateTime currentTime = DateTime.Now;

                                tracker.TotalCharacters = characters;
                                tracker.NewCharacters = tracker.TotalCharacters - tracker.PreviousCharacters;
                                tracker.ClosingTime = currentTime;

                                if (dataManager.Search(tracker.ProjectName, tracker.FileName) != null)
                                {
                                    dataManager.Update(tracker);
                                }
                                else
                                {
                                    dataManager.Insert(tracker);
                                }
                            }
                            outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                            customPane.OutputStringThreadSafe($"\nFile: {tracker.FileName} closed.");
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                customPane.OutputStringThreadSafe($"\nAn error has occurred: {ex.Message}");
            }
        }
        private static async void OnLineChanged(TextPoint startPoint, TextPoint endPoint, int hint)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            try
            {
                if (startPoint.Parent != null & startPoint.Parent.Parent != null)
                {
                    Tracker tracker = Instance.trackers.Find(t =>
                    {
                        ThreadHelper.ThrowIfNotOnUIThread();
                        return t.FileName == startPoint.Parent.Parent.ActiveWindow.Document.Name && t.Path == startPoint.Parent.Parent.ActiveWindow.Document.FullName;
                    });
                    if (tracker != null)
                    {
                        string modifiedText = startPoint.CreateEditPoint().GetText(endPoint);
                        if (modifiedText.Length == 1 && ContainsAlphanumericCharacter(modifiedText))
                        {
                            previousKeyPressed = modifiedText;
                            tracker.NewKeysPressed++;
                            tracker.TotalKeysPressed++;

                            outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                            customPane.OutputStringThreadSafe($"\nCharacter entered by the user in {tracker.FileName}");
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                outputWindow.GetPane(ref outputGuid, out IVsOutputWindowPane customPane);
                customPane.OutputStringThreadSafe($"\nAn error has ocurred: {ex.Message}");
            }
        }
        private static bool ContainsAlphanumericCharacter(string text)
        {
            return !string.IsNullOrEmpty(text) && text.Any(char.IsLetterOrDigit);
        }
        #endregion

        #region REPORTER
        private static void ShowStatistics(object sender, EventArgs e)
        {
            List<Tracker> data = dataManager.ReadAll();
            if (data.Count > 0)
            {
                int totalFiles = data.Count;
                int totalCharacters = 0;
                int totalCharactersByUser = 0;
                int totalCharactersByAI = 0;

                foreach (Tracker tracker in data)
                {
                    totalCharacters += tracker.TotalCharacters;
                    totalCharactersByUser += tracker.TotalKeysPressed;
                }

                totalCharactersByAI += totalCharacters - totalCharactersByUser;

                double totalCharactersByUserPercent = (double)totalCharactersByUser / totalCharacters;
                double totalCharactersByAIPercent = (double)totalCharactersByAI / totalCharacters;

                Report report = new Report(user, totalFiles, totalCharacters, totalCharactersByUser, totalCharactersByAI, totalCharactersByUserPercent, totalCharactersByAIPercent);
                report.Show();
            }
            else
            {
                string message = "Data not found.";

                ThreadHelper.ThrowIfNotOnUIThread();
                IVsUIShell uiShell = Instance.package.GetService<SVsUIShell, IVsUIShell>();
                Guid clsid = Guid.Empty;
                uiShell.ShowMessageBox(
                    0,
                    ref clsid,
                    $"DevTimeMonitor\n------------------------------------------------------------------\nDate: {DateTime.Now:dd-MM-yyyy hh:mm:ss}\nEnviroment: {user}\n------------------------------------------------------------------",
                    string.Format(CultureInfo.CurrentCulture,
                    message,
                    Instance.package.ToString()),
                    string.Empty,
                    0,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK,
                    OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                    OLEMSGICON.OLEMSGICON_INFO,
                    0,
                    out int result);
            }
        }
        #endregion
    }
}