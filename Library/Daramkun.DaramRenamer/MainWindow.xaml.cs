﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Daramkun.DaramRenamer.Processors.Extension;
using Daramkun.DaramRenamer.Processors.Filename;
using Daramkun.DaramRenamer.Processors.Number;
using Daramkun.DaramRenamer.Processors.FilePath;
using Daramkun.DaramRenamer.Processors.Date;
using Daramkun.DaramRenamer.Processors.Tag;
using System.Threading;
using System.Windows.Media;
using Daramkun.DaramRenamer.Processors;
using System.Windows.Threading;
using Daramee.DaramCommonLib;
using Daramee.Winston.Dialogs;
using System.ComponentModel;
using Daramkun.DaramRenamer.Extension;
using Daramee.Winston.File;
using System.Collections.Concurrent;
using System.Text;

namespace Daramkun.DaramRenamer
{
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		#region Commands
		public static RoutedCommand CommandOpenFiles = new RoutedCommand ();
		public static RoutedCommand CommandClearList = new RoutedCommand ();
		public static RoutedCommand CommandApplyFile = new RoutedCommand ();
		public static RoutedCommand CommandUndoWorks = new RoutedCommand ();
		public static RoutedCommand CommandRedoWorks = new RoutedCommand ();
		public static RoutedCommand CommandUpperItem = new RoutedCommand ();
		public static RoutedCommand CommandLowerItem = new RoutedCommand ();
		public static RoutedCommand CommandItemsSort = new RoutedCommand ();

		private void CommandOpenFiles_Executed ( object sender, ExecutedRoutedEventArgs e ) { Menu_System_Open ( sender, e ); }
		private void CommandClearList_Executed ( object sender, ExecutedRoutedEventArgs e ) { Menu_System_Clear ( sender, e ); }
		private void CommandApplyFile_Executed ( object sender, ExecutedRoutedEventArgs e ) { Menu_System_Apply ( sender, e ); }

		private void CommandUndoWorks_Executed ( object sender, ExecutedRoutedEventArgs e ) { Menu_System_Undo ( sender, e ); }
		private void CommandRedoWorks_Executed ( object sender, ExecutedRoutedEventArgs e ) { Menu_System_Redo ( sender, e ); }

		private void CommandApplyCanc_Executed ( object sender, ExecutedRoutedEventArgs e )
		{ while ( !UndoManager.IsUndoStackEmpty ) Menu_System_Undo ( sender, e ); }

		private void CommandUpperItem_Executed ( object sender, ExecutedRoutedEventArgs e ) { Menu_System_ItemUp ( sender, e ); }
		private void CommandLowerItem_Executed ( object sender, ExecutedRoutedEventArgs e ) { Menu_System_ItemDown ( sender, e ); }
		private void CommandItemsSort_Executed ( object sender, ExecutedRoutedEventArgs e ) { Menu_System_ItemSort ( sender, e ); }
		#endregion

		public static MainWindow SharedWindow { get; private set; }

		Optionizer<SaveData> option;
		UpdateChecker updateChecker;
		ToolBarIconExtension toolBarIcon;

		public UndoManager<ObservableCollection<FileInfo>> UndoManager { get; } = new UndoManager<ObservableCollection<FileInfo>> ();
		public bool UndoManagerHasUndoStackItem => !UndoManager.IsUndoStackEmpty;
		public bool UndoManagerHasRedoStackItem => !UndoManager.IsRedoStackEmpty;

		public RenameMode RenameMode { get { return option.Options.RenameMode; } set { option.Options.RenameMode = value; } }
		public bool HardwareAccelerationMode { get { return option.Options.HardwareAccelerationMode; } set { option.Options.HardwareAccelerationMode = value; } }
		public bool AutomaticFilenameFix { get { return option.Options.AutomaticFilenameFix; } set { option.Options.AutomaticFilenameFix = value; } }
		public bool AutomaticListCleaning { get { return option.Options.AutomaticListCleaning; } set { option.Options.AutomaticListCleaning = value; } }
		public bool Overwrite { get { return option.Options.Overwrite; } set { option.Options.Overwrite = value; } }
		public bool SaveWindowState { get { return option.Options.SaveWindowState; } set { option.Options.SaveWindowState = value; } }

		public MainWindow ()
		{
			SharedWindow = this;

			updateChecker = new UpdateChecker ( "{0}.{1}{2}{3}" );
			
			option = new Optionizer<SaveData> ( "DARAM WORLD", "DaramRenamer" );
			toolBarIcon = new ToolBarIconExtension ( option.Options.ToolBarIconPack );
			
			InitializeComponent ();

			if ( option.Options.SaveWindowState )
			{
				Left = option.Options.Left;
				Top = option.Options.Top;
				Width = option.Options.Width;
				Height = option.Options.Height;
				WindowState = option.Options.WindowState;
			}

			optionRenameMode.SelectedIndex = option.Options.RenameModeInteger;
			
			Version currentVersion = Assembly.GetEntryAssembly ().GetName ().Version;
			Title = $"{StringTable.SharedStrings [ "daram_renamer" ]} - v{currentVersion.Major}.{currentVersion.Minor}{currentVersion.Build}0";
			
			translationAuthor.Text = StringTable.SharedTable.Author.Contact != null ?
				$"{StringTable.SharedTable.Author.Author}<{StringTable.SharedTable.Author.Contact}> - {StringTable.SharedTable.CurrentCulture}" :
				$"{StringTable.SharedTable.Author.Author} - {StringTable.SharedTable.CurrentCulture}";

			/*UndoManager<ObservableCollection<FileInfo>> restored = UndoManager<ObservableCollection<FileInfo>>.Restore ();
			if ( restored != null )
			{
				undoManager = restored;
				Menu_System_Undo ( this, null );
			}*/
			UndoManager.UpdateUndo += ( sender, e ) => { PC ( nameof ( UndoManagerHasUndoStackItem ) ); PC ( nameof ( UndoManagerHasRedoStackItem ) ); };
			UndoManager.UpdateRedo += ( sender, e ) => { PC ( nameof ( UndoManagerHasUndoStackItem ) ); PC ( nameof ( UndoManagerHasRedoStackItem ) ); };

			listViewFiles.ItemsSource = FileInfo.Files;
		}

		private async void Window_Loaded ( object sender, RoutedEventArgs e )
		{
			if ( await updateChecker.CheckUpdate () == true )
			{
				Title = $"{Title} - [{StringTable.SharedStrings [ "available_update" ]}]";
			}
		}

		public static TaskDialogResult MessageBox ( string message, string content, TaskDialogIcon icon,
			TaskDialogCommonButtonFlags commonButtons, params string [] buttons )
		{
			TaskDialogButton [] tdButtons = buttons != null ? TaskDialogButton.Cast ( buttons ) : null;
			
			TaskDialog taskDialog = new TaskDialog
			{
				Title = StringTable.SharedStrings [ "daram_renamer" ],
				MainInstruction = message,
				Content = content,
				MainIcon = icon,
				CommonButtons = commonButtons,
				Buttons = tdButtons,
			};
            return taskDialog.Show(MainWindow.SharedWindow);
		}

		public void AddItem ( string s, bool directoryMode = false )
		{
			if ( System.IO.File.Exists ( s ) || directoryMode )
			{
				var fileInfo = new FileInfo ( s );
				if ( !FileInfo.Files.Contains ( fileInfo ) )
					FileInfo.Files.Add ( fileInfo );
			}
			else
			{
				foreach ( string ss in FilesEnumerator.EnumerateFiles ( s, "*.*", false ) )
					AddItem ( ss, directoryMode );
			}
		}

		public void ShowPopup<T> ( params object [] args ) where T : IProcessor
		{
			T processor = Activator.CreateInstance<T> ();
			if ( processor is ManualEditProcessor )
			{
				( processor as ManualEditProcessor ).ChangeName = ( args [ 0 ] as FileInfo ).ChangedFilename;
				( processor as ManualEditProcessor ).ChangePath = ( args [ 0 ] as FileInfo ).ChangedPath;
				( processor as ManualEditProcessor ).ProcessingFileInfo = args [ 0 ] as FileInfo;
			}
			ISubWindow window = ( processor is BatchProcessor )
				? new SubWindow_Batch () as ISubWindow
				: new SubWindow ( processor );
			UserControl windowControl = window as UserControl;
			window.OKButtonClicked += SubWindow_OKButtonClicked;
			window.CancelButtonClicked += SubWindow_CancelButtonClicked;
			windowControl.VerticalAlignment = VerticalAlignment.Center;
			windowControl.HorizontalAlignment = HorizontalAlignment.Center;
			overlayWindowContainer.Children.Add ( windowControl );
			overlayWindowGrid.Visibility = Visibility.Visible;
		}

		public void ClosePopup ( bool apply = false )
		{
			overlayWindowGrid.Visibility = Visibility.Hidden;
			if ( apply )
			{
				UndoManager.SaveToUndoStack ( FileInfo.Files );
				var processor = ( overlayWindowContainer.Children [ 0 ] as ISubWindow ).Processor;
				if ( processor is ManualEditProcessor )
				{
					processor.Process ( ( processor as ManualEditProcessor ).ProcessingFileInfo );
				}
				else
				{
					if ( !processor.CannotMultithreadProcess )
						Parallel.ForEach<FileInfo> ( FileInfo.Files, ( fileInfo ) => processor.Process ( fileInfo ) );
					else foreach ( var fileInfo in FileInfo.Files ) processor.Process ( fileInfo );
				}
			}
			overlayWindowContainer.Children.Clear ();
		}

		private void Item_DoubleClick ( object sender, RoutedEventArgs e )
		{
			if ( ( sender as ListViewItem ).Content == null ) return;
			FileInfo info = ( sender as ListViewItem ).Content as FileInfo;
			ShowPopup<ManualEditProcessor> ( info );
		}

		private void ListViewFiles_DragEnter ( object sender, DragEventArgs e )
		{
			if ( e.Data.GetDataPresent ( DataFormats.FileDrop ) ) e.Effects = DragDropEffects.None;
		}

		private void ListViewFiles_Drop ( object sender, DragEventArgs e )
		{
			if ( e.Data.GetDataPresent ( DataFormats.FileDrop ) )
			{
				var temp = e.Data.GetData ( DataFormats.FileDrop ) as string [];
				bool hasDirectory = false;
				foreach ( string filename in temp )
				{
					if ( File.GetAttributes ( filename ).HasFlag ( FileAttributes.Directory ) && filename.Length > 3 )
						hasDirectory = true;
				}

				bool directoryMode = false;
				if ( hasDirectory )
				{
					var result = MessageBox ( StringTable.SharedStrings["dnd_directory_question"], StringTable.SharedStrings [ "dnd_directory_question_more" ],
						TaskDialogNativeIcon.Warning, TaskDialogCommonButtonFlags.Cancel,
						StringTable.SharedStrings [ "dnd_button_add_directory" ], StringTable.SharedStrings [ "dnd_button_iterate_files" ] );
					if ( result.Button == TaskDialogResult.Cancel )
						return;
					directoryMode = result.Button == 101;
				}

				UndoManager.SaveToUndoStack ( FileInfo.Files );

				foreach ( string s in from b in temp orderby b select b )
				{
					AddItem ( s, ( s.Length <= 3 || !System.IO.File.GetAttributes ( s ).HasFlag ( FileAttributes.Directory ) ) ? false : directoryMode );
				}
			}
		}

		private void ListViewFiles_KeyUp ( object sender, KeyEventArgs e )
		{
			if ( e.Key == Key.Delete )
			{
				UndoManager.SaveToUndoStack ( FileInfo.Files );

				List<FileInfo> tempFileInfos = new List<FileInfo> ();
				foreach ( FileInfo fileInfo in listViewFiles.SelectedItems ) tempFileInfos.Add ( fileInfo );
				foreach ( FileInfo fileInfo in tempFileInfos ) FileInfo.Files.Remove ( fileInfo );
				if ( FileInfo.Files.Count == 0 ) { UndoManager.ClearAll (); }
			}
		}

		private void Menu_System_Open ( object sender, RoutedEventArgs e )
		{
			Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
			{
				Title = StringTable.SharedStrings [ "open_files" ],
				Filter = StringTable.SharedStrings [ "all_files" ],
				Multiselect = true
			};
			if ( openFileDialog.ShowDialog () == false ) return;

			UndoManager.SaveToUndoStack ( FileInfo.Files );

			foreach ( string s in from s in openFileDialog.FileNames orderby s select s )
				AddItem ( s );
		}

		private void Menu_System_Clear ( object sender, RoutedEventArgs e )
		{
			UndoManager.ClearAll ();
			FileInfo.Files.Clear ();
		}

		private void Menu_System_Apply ( object sender, RoutedEventArgs e )
		{
			UndoManager.ClearUndoStack ();

			progressBar.Foreground = Brushes.Green;
			progressBar.Maximum = FileInfo.Files.Count;
			progressBar.Value = 0;

			bool failed = false;
			FileInfo.Apply ( AutomaticFilenameFix, option.Options.RenameMode, Overwrite, ( fileInfo, errorCode ) =>
			{
				Dispatcher.BeginInvoke ( ( Action ) ( () => { ++progressBar.Value; } ) );
				if ( errorCode != ErrorCode.NoError )
					failed = true;
			} );

			if ( failed )
				progressBar.Foreground = Brushes.Red;

			Application.Current.Dispatcher.Invoke ( DispatcherPriority.Background, new ThreadStart ( delegate { } ) );

			MessageBox ( StringTable.SharedStrings [ "applied" ], string.Format ( StringTable.SharedStrings [ "applied_message" ],
				progressBar.Value, progressBar.Maximum ),
				TaskDialogIcon.Information, TaskDialogCommonButtonFlags.OK );

			if ( !failed && option.Options.AutomaticListCleaning )
			{
				UndoManager.SaveToUndoStack ( FileInfo.Files );
				FileInfo.Files.Clear ();
			}
		}

		private void Menu_System_Undo ( object sender, RoutedEventArgs e )
		{
			if ( UndoManager.IsUndoStackEmpty )
				return;

			UndoManager.SaveToRedoStack ( FileInfo.Files );

			var data = UndoManager.LoadFromUndoStack () ?? throw new Exception ();
			listViewFiles.ItemsSource = FileInfo.Files = data;
		}

		private void Menu_System_Redo ( object sender, RoutedEventArgs e )
		{
			if ( UndoManager.IsRedoStackEmpty )
				return;

			UndoManager.SaveToUndoStack ( FileInfo.Files, false );

			var data = UndoManager.LoadFromRedoStack () ?? throw new Exception ();
			listViewFiles.ItemsSource = FileInfo.Files = data;
		}

		private void Menu_System_ItemUp ( object sender, RoutedEventArgs e )
		{
			if ( listViewFiles.SelectedItems.Count == 0 ) return;
			UndoManager.SaveToUndoStack ( FileInfo.Files );
			foreach ( FileInfo fileInfo in listViewFiles.SelectedItems )
			{
				int lastIndex = FileInfo.Files.IndexOf ( fileInfo );
				if ( lastIndex == 0 ) continue;
				FileInfo.Files.Move ( lastIndex, lastIndex - 1 );
			}
		}

		private void Menu_System_ItemDown ( object sender, RoutedEventArgs e )
		{
			if ( listViewFiles.SelectedItems.Count == 0 ) return;
			UndoManager.SaveToUndoStack ( FileInfo.Files );
			foreach ( FileInfo fileInfo in listViewFiles.SelectedItems )
			{
				int lastIndex = FileInfo.Files.IndexOf ( fileInfo );
				if ( lastIndex == FileInfo.Files.Count - 1 ) continue;
				FileInfo.Files.Move ( lastIndex, lastIndex + 1 );
			}
		}

		private void Menu_System_ItemSort ( object sender, RoutedEventArgs e )
		{
			UndoManager.SaveToUndoStack ( FileInfo.Files );
			FileInfo.Sort ( FileInfo.Files );
		}

		private async void Menu_System_CheckUpdate ( object sender, RoutedEventArgs e )
		{
			if ( await updateChecker.CheckUpdate () == true )
			{
				if ( MessageBox ( StringTable.SharedStrings [ "update_exist" ], StringTable.SharedStrings [ "current_old" ],
								TaskDialogIcon.Information, TaskDialogCommonButtonFlags.OK, StringTable.SharedStrings [ "button_download" ] ).
								Button == 101 )
					updateChecker.ShowDownloadPage ();
			}
			else
			{
				MessageBox ( StringTable.SharedStrings [ "no_update" ], StringTable.SharedStrings [ "current_stable" ],
								TaskDialogIcon.Information, TaskDialogCommonButtonFlags.OK );
			}
		}

		private void ComboBox_SelectionChanged ( object sender, SelectionChangedEventArgs e )
		{
			option.Options.RenameModeInteger = ( sender as ComboBox ).SelectedIndex;
		}

		private void SubWindow_OKButtonClicked ( object sender, RoutedEventArgs e) { ClosePopup ( true ); }
		private void SubWindow_CancelButtonClicked ( object sender, RoutedEventArgs e ) { ClosePopup (); }

		private void ReplacePlainText_Click ( object sender, RoutedEventArgs e ) { ShowPopup<ReplacePlainProcessor> (); }
		private void ReplaceRegex_Click ( object sender, RoutedEventArgs e ) { ShowPopup<ReplaceRegexpProcessor> (); }
		private void RearrangeRegex_Click ( object sender, RoutedEventArgs e ) { ShowPopup<RearrangeRegexpProcessor> (); }
		private void ConcatText_Click ( object sender, RoutedEventArgs e ) { ShowPopup<ConcatenateProcessor> (); }
		private void Trimming_Click ( object sender, RoutedEventArgs e ) { ShowPopup<TrimmingProcessor> (); }
		private void DeleteBlock_Click ( object sender, RoutedEventArgs e ) { ShowPopup<DeleteBlockProcessor> (); }
		private void DeleteText_Click ( object sender, RoutedEventArgs e )
		{
			UndoManager.SaveToUndoStack ( FileInfo.Files );
			Parallel.ForEach<FileInfo> ( FileInfo.Files, ( fileInfo ) => new DeleteFilenameProcessor ().Process ( fileInfo ) );
		}
		private void Substring_Click ( object sender, RoutedEventArgs e ) { ShowPopup<SubstringProcessor> (); }
		private void Castcast_Click ( object sender, RoutedEventArgs e ) { ShowPopup<CasecastProcessor> (); }

		private void AddExtension_Click ( object sender, RoutedEventArgs e ) { ShowPopup<AddExtensionProcessor> (); }
		private void AddExtensionAutomatically_Click ( object sender, RoutedEventArgs e )
		{
			UndoManager.SaveToUndoStack ( FileInfo.Files );
			Parallel.ForEach<FileInfo> ( FileInfo.Files, ( fileInfo ) => new AddExtensionAutomatedProcessor ().Process ( fileInfo ) );
		}
		private void RemoveExtension_Click ( object sender, RoutedEventArgs e )
		{
			UndoManager.SaveToUndoStack ( FileInfo.Files );
			Parallel.ForEach<FileInfo> ( FileInfo.Files, ( fileInfo ) => new DeleteExtensionProcessor ().Process ( fileInfo ) );
		}
		private void ChangeExtension_Click ( object sender, RoutedEventArgs e ) { ShowPopup<ReplaceExtensionProcessor> (); }
		private void CastcastExtension_Click ( object sender, RoutedEventArgs e ) { ShowPopup<CasecastExtensionProcessor> (); }

		private void DeleteWithoutNumbers_Click ( object sender, RoutedEventArgs e ) { ShowPopup<DeleteWithoutNumbersProcessor> (); }
		private void MatchingNumberCount_Click ( object sender, RoutedEventArgs e ) { ShowPopup<NumberCountMatchProcessor> (); }
		private void AddIndexNumbers_Click ( object sender, RoutedEventArgs e ) { ShowPopup<AddIndexNumberProcessor> (); }
		private void IncreaseDecreaseNumbers_Click ( object sender, RoutedEventArgs e ) { ShowPopup<IncreaseDecreaseNumbersProcessor> (); }

		private void AddDate_Click ( object sender, RoutedEventArgs e ) { ShowPopup<AddDateProcessor> (); }
		private void DeleteDate_Click ( object sender, RoutedEventArgs e )
		{
			UndoManager.SaveToUndoStack ( FileInfo.Files );
			Parallel.ForEach<FileInfo> ( FileInfo.Files, ( fileInfo ) => new DeleteDateProcessor ().Process ( fileInfo ) );
		}
		private void IncreaseDecreaseDate_Click ( object sender, RoutedEventArgs e ) { /* TODO */ }

		private void ChangePath_Click ( object sender, RoutedEventArgs e ) { ShowPopup<ChangePathProcessor> (); }
		private void MovePathRelative_Click ( object sender, RoutedEventArgs e ) { /* TODO */ }

		private void AddMediaTag_Click ( object sender, RoutedEventArgs e ) { ShowPopup<AddMediaTagProcessor> (); }
		private void AddDocumentTag_Click ( object sender, RoutedEventArgs e ) { ShowPopup<AddDocumentTagProcessor> (); }
		private void AddFileHash_Click ( object sender, RoutedEventArgs e ) { ShowPopup<AddHashProcessor> (); }

		private void BatchProcess_Click ( object sender, RoutedEventArgs e )
		{
			ShowPopup<BatchProcessor> ();
		}

		private void LicenseTextBox_Loaded ( object sender, RoutedEventArgs e )
		{
			using ( Stream license = Assembly.GetEntryAssembly ().GetManifestResourceStream ( "Daramkun.DaramRenamer.Resources.LICENSE" ) )
			{
				using ( StreamReader reader = new StreamReader ( license, Encoding.UTF8, true, 1024, true ) )
				{
					licenseTextBox.Text = reader.ReadToEnd ();
				}
			}
		}

		private void Window_Activated ( object sender, EventArgs e )
		{
			foreach ( var child in overlayWindowContainer.Children )
			{
				if ( child is SubWindow_Batch )
				{
					( child as SubWindow_Batch ).Activated ();
				}
			}
		}

		private void Window_Closing ( object sender, System.ComponentModel.CancelEventArgs e )
		{
			if ( option.Options.SaveWindowState )
			{
				option.Options.Left = Left;
				option.Options.Top = Top;
				option.Options.Width = Width;
				option.Options.Height = Height;
				option.Options.WindowState = WindowState;
			}
			Optionizer<SaveData>.SharedOptionizer.Save ();
		}

		private void Hyperlink_RequestNavigate ( object sender, System.Windows.Navigation.RequestNavigateEventArgs e )
		{
			Process.Start ( new ProcessStartInfo ( e.Uri.AbsoluteUri ) );
			e.Handled = true;
		}
		
		public event PropertyChangedEventHandler PropertyChanged;
		private void PC ( string name ) { PropertyChanged?.Invoke ( this, new PropertyChangedEventArgs ( name ) ); }
	}
}
