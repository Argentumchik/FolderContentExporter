using FolderContentExporter.Commands;
using FolderContentExporter.Dto;
using FolderContentExporter.Interfaces;
using FolderContentExporter.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FolderContentExporter.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IFolderDialogService _folderDialogService;
        private readonly IFileSystemService _fileSystemService;
        private readonly IFileExportService _fileExportService;

        private string _selectedFolder = string.Empty;
        private string _exportFileName = "file";
        private bool _subfoldersIncluded;
        private bool _isLoading;
        private string _isCancelled = "Hidden";
        private int _progress;
        private int _totalFiles = 1;
        private string _textBack;

        private ExportMode _selectedExportMode;
        private CancellationTokenSource? _cts;

        public Array ExportModes => Enum.GetValues<ExportMode>();

        public string TextBack
        {
            get => _textBack;
            set
            {
                _textBack = value;
                OnPropertyChanged();
            }
        }
        public string SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                _selectedFolder = value;
                OnPropertyChanged();

                LoadFileCommand?.RaiseCanExecuteChanged();
            }
        }
        public bool SubfoldersIncluded 
        { 
            get => _subfoldersIncluded;
            set
            {
                _subfoldersIncluded = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(_selectedFolder))
                {
                    _ = LoadFiles();
                }
            }
        }
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                LoadFileCommand?.RaiseCanExecuteChanged();
            }
        }
        public string IsCancelled
        {
            get => _isCancelled;
            set
            {
                _isCancelled = value;
                OnPropertyChanged();
            }
        }
        public ExportMode SelectedExportMode
        {
            get => _selectedExportMode;
            set
            {
                _selectedExportMode = value;
                OnPropertyChanged();
            }
        }
        public string ExportFileName
        {
            get => _exportFileName;
            set
            {
                _exportFileName = value;
                OnPropertyChanged();
            }
        }
        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }
        public int TotalFiles
        {
            get => _totalFiles;
            set
            {
                _totalFiles = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TextFileItem> Files { get; } = [];

        public ICommand SelectFolderCommand { get; }
        public RelayCommandAsync LoadFileCommand { get; }
        public ICommand ExportFileCommand { get; }
        public ICommand CancelCommand { get; }

        public MainViewModel(IFolderDialogService folderDialogService, IFileSystemService fileSystemService, IFileExportService fileExportService)
        {
            _folderDialogService = folderDialogService;
            _fileSystemService = fileSystemService;
            _fileExportService = fileExportService;

            SelectFolderCommand = new RelayCommand(SelectFolder);
            LoadFileCommand = new RelayCommandAsync(LoadFiles, CanLoadFiles);
            ExportFileCommand = new RelayCommand(ExportFile, CanExportFiles);
            CancelCommand = new RelayCommand(Cancel, () => IsLoading);
        }

        private void SelectFolder()
        {
            SelectedFolder = _folderDialogService.LoadFolder();
        }
        private void Cancel()
        {
            _cts?.Cancel();
        }

        private async Task LoadFiles()
        {
            Files.Clear();
            TotalFiles = await _fileSystemService.TotalFilesAsync(SelectedFolder, SubfoldersIncluded);
            Progress = 0;
            IsLoading = true;
            IsCancelled = "Hidden";

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                await Task.Run(async () =>
                {
                    int progressed = 0;

                    await foreach (var file in _fileSystemService.GetFilesAsync(SelectedFolder, SubfoldersIncluded, token))
                    {
                        progressed++;
                        token.ThrowIfCancellationRequested();

                        await Application.Current.Dispatcher.InvokeAsync(() =>
                        {
                            Files.Add(file);
                            if (progressed % 50 == 0 || progressed == TotalFiles)
                            {
                                Progress = progressed;
                            }
                        });
                    }
                }, token);
            }
            catch (OperationCanceledException)
            {
                IsCancelled = "Visible";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                IsLoading = false;
                _cts.Dispose();
                _cts = null;
            }
        }

        private void ExportFile()
        {
            var dialog = new ExportDialogWindow();
            var vm = new ExportDialogViewModel();
            dialog.DataContext = vm;

            if (dialog.ShowDialog() == true)
            {
                var options = vm.BuildOptions();

                var path = _folderDialogService.LoadFolder();

                try
                {
                    _fileExportService.ExportFiles(Files, path, options);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred during export: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show("Files exported successfully.", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private bool CanLoadFiles()
        {
            return !string.IsNullOrEmpty(SelectedFolder) && Directory.Exists(SelectedFolder);
        }
        private bool CanExportFiles()
        {
            return !string.IsNullOrEmpty(ExportFileName) && Files.Count > 0;
        }
    }
}
