using Daramee.Winston.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MagicFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ListViewFiles.ItemsSource = FileInfo.Files;

            ((INotifyCollectionChanged)ListViewFiles.ItemsSource).CollectionChanged += new NotifyCollectionChangedEventHandler(ListViewFiles_CollectionChanged);
            var totalSize = FileInfo.Files.Sum(f => f.FileSize);
            FilesCounterText = string.Format(StringTable.SharedStrings["FilesCounterText"], FileInfo.Files.Count, FormatFileSize(totalSize));
            Label_FilesCounter.Content = FilesCounterText;
            Label_FilesCounter.DataContext = FilesCounterText;
        }

        public string FormatFileSize(long fileSize)
        {
            string sizeText = string.Empty;
            if (fileSize < 1024)
            {
                return fileSize.ToString() + " B";
            }
            else if (fileSize < (1024 * 1024))
            {
                var temp = fileSize / 1024f;
                sizeText = temp.ToString("0.00", CultureInfo.CurrentCulture);
                return sizeText + " KB";
            }
            else if (fileSize < (1024 * 1024 * 1024))
            {
                var temp = fileSize / (1024f * 1024f);
                sizeText = temp.ToString("0.00", CultureInfo.CurrentCulture);
                return sizeText + " MB";
            }
            else
            {
                var temp = fileSize / (1024f * 1024f * 1024f);
                sizeText = temp.ToString("0.00", CultureInfo.CurrentCulture);
                return sizeText + " GB";
            }
        }

        public void ListViewFiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var totalSize = FileInfo.Files.Sum(f => f.FileSize);
            //FilesCounterText = string.Format(StringTable.SharedStrings["FilesCounterText"], FileInfo.Files.Count);
            string arg1 = FormatFileSize(totalSize);
            Label_FilesCounter.Content = string.Format(StringTable.SharedStrings["FilesCounterText"], FileInfo.Files.Count, arg1);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        internal static string GetCopyrightString()
        {
            var copyrights = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), true);
            return copyrights is { Length: > 0 } ? (copyrights[0] as AssemblyCopyrightAttribute)?.Copyright ?? string.Empty : string.Empty;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Activated(object sender, EventArgs e)
        {

        }

        private void BatchProcess_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_System_Redo(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_System_Undo(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_Licenses_Click(object sender, RoutedEventArgs e)
        {
            new LicenseWindow() { Owner = this }.ShowDialog();
        }

        private void Menu_About_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow() { Owner = this }.ShowDialog();
        }
        private readonly UndoManager _undoManager = new();
        private readonly string FilesCounterText = string.Empty;
        private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new()
            {
                Title = StringTable.SharedStrings["FileDialogTitleOpenFiles"],
                Filter = StringTable.SharedStrings["FileDialogFilterAllFiles"],
                Multiselect = true
            };
            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }

            _undoManager.SaveToUndoStack(FileInfo.Files);

            foreach (string s in from s in openFileDialog.FileNames orderby s select s)
            {
                AddItem(s);
            }
        }

        private void MenuFileFolderOpen_Click(object sender, RoutedEventArgs e)
        {
            var openFolderDialog = new OpenFolderDialog
            {
                Title = StringTable.SharedStrings["FileDialogTitleOpenFiles"],
                AllowMultiSelection = true
            };
            if (openFolderDialog.ShowDialog() == false)
            {
                return;
            }

            _undoManager.SaveToUndoStack(FileInfo.Files);

            foreach (string s in from s in openFolderDialog.FileNames orderby s select s)
            {
                AddItem(s, true);
            }
        }

        public void AddItem(string path, bool directoryMode = false)
        {
            if (FileInfo.FileOperator.FileExists(path) || directoryMode)
            {
                FileInfo fileInfo = new(path);
                if (!FileInfo.Files.Contains(fileInfo))
                {
                    FileInfo.Files.Add(fileInfo);
                }
            }
            else
            {
                foreach (string file in FileInfo.FileOperator.GetFiles(path, false))
                {
                    AddItem(file);
                }
            }
        }

        private void ListViewFiles_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            if (e.Data.GetData(DataFormats.FileDrop) is not string[] temp)
            {
                return;
            }

            bool hasDirectory = false;
            foreach (string filename in temp)
            {
                if (File.GetAttributes(filename).HasFlag(FileAttributes.Directory) && filename.Length > 3)
                {
                    hasDirectory = true;
                }
            }

            bool directoryMode = false;
            if (hasDirectory)
            {
                TaskDialogResult result = MessageBox(StringTable.SharedStrings["DragAndDrop_DirectoryQuestion"],
                    StringTable.SharedStrings["DragAndDrop_DirectoryQuestionDescription"],
                    TaskDialogIcon.Warning, TaskDialogCommonButtonFlags.Cancel,
                    StringTable.SharedStrings["DragAndDrop_ButtonAddDirectory"],
                    StringTable.SharedStrings["DragAndDrop_ButtonIterateFiles"]);
                if (result.Button == TaskDialogResult.Cancel)
                {
                    return;
                }

                directoryMode = result.Button == 101;
            }

            _undoManager.SaveToUndoStack(FileInfo.Files);

            foreach (string s in from b in temp orderby b select b)
            {
                AddItem(s, s.Length > 3 && File.GetAttributes(s).HasFlag(FileAttributes.Directory) && directoryMode);
            }
        }

        private void ListViewFiles_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Delete)
            {
                return;
            }

            _undoManager.SaveToUndoStack(FileInfo.Files);

            foreach (FileInfo fileInfo in ListViewFiles.SelectedItems.Cast<FileInfo>().ToList())
            {
                FileInfo.Files.Remove(fileInfo);
            }
        }

        public static TaskDialogResult MessageBox(string message, string content, TaskDialogIcon icon,
           TaskDialogCommonButtonFlags commonButtons, params string[] buttons)
        {
            TaskDialogButton[] taskDialogButtons = buttons != null ? TaskDialogButton.Cast(buttons) : null;

            TaskDialog taskDialog = new()
            {
                Title = StringTable.SharedStrings["MagicFile"],
                MainInstruction = message,
                Content = content,
                MainIcon = icon,
                CommonButtons = commonButtons,
                Buttons = taskDialogButtons,
            };
            return taskDialog.Show(Application.Current.MainWindow);
        }
        private void Item_DoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void ToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            ToolBar toolBar = sender as ToolBar;
            FrameworkElement overflowGrid = toolBar.Template.FindName("OverflowGrid", toolBar) as FrameworkElement;
            if (overflowGrid != null)
            {
                overflowGrid.Visibility = Visibility.Collapsed;
            }

            FrameworkElement mainPanelBorder = toolBar.Template.FindName("MainPanelBorder", toolBar) as FrameworkElement;
            if (mainPanelBorder != null)
            {
                mainPanelBorder.Margin = new Thickness(0);
            }
        }

        #region 规则事件...

        private void AddRule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UpRule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DownRule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RemoveRule_Click(object sender, RoutedEventArgs e)
        {

        }

        #endregion

        private static async Task<bool> IsAdministrator()
        {
            return await Task.Run(() => new WindowsPrincipal(WindowsIdentity.GetCurrent())
                .IsInRole(WindowsBuiltInRole.Administrator));
        }

        internal static string GetVersionString()
        {
            var programVersion = Assembly.GetExecutingAssembly().GetName().Version;
            return programVersion != null
                ? $"{programVersion.Major}.{programVersion.Minor}.{programVersion.Build}"
                : "UNKNOWN VERSION";
        }


        //public async void RefreshTitle()
        //{
        //    Title = $"{Strings.Instance["DaramRenamer"]} - {Strings.Instance["Version"]} {GetVersionString()}";

        //    if (!Environment.Is64BitProcess)
        //        Title += " - [32-Bit]";
        //    if (await IsAdministrator())
        //        Title = $"{Title} - [{Strings.Instance["Administrator"]}]";

        //    var updateInfo = await CheckUpdate();

        //    Title = updateInfo switch
        //    {
        //        true => $"{Title} - [{Strings.Instance["NewLatestVersionAvailable"]}]",
        //        null => $"{Title} - [{Strings.Instance["UpdateCheckError"]}]",
        //        _ => Title
        //    };
        //}

        internal static async Task<bool?> CheckUpdate()
        {
            return await Task.Run<bool?>(() =>
            {
                var updateInformation = UpdateInformationBank.GetUpdateInformation(TargetPlatform.Windows);
                if (updateInformation != null)
                    return updateInformation.Value.StableLatestVersion != GetVersionString();
                return null;
            });
        }

        private void ListViewFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Menu_Unmark_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_Mark_Click(object sender, RoutedEventArgs e)
        {

        }


        private void Menu_ClearAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_ClearFailed_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_ClearInvalid_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_RemoveSelected_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_SelectAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_InvertSelection_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_InvertMarking_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_MoveDown_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_MoveUp_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Menu_AnalyzeName_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
