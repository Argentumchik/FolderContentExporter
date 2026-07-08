using FolderContentExporter.Commands;
using FolderContentExporter.Dto;
using FolderContentExporter.Enums;
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
        private readonly IErrorMapper _errorMapper;

        private string _selectedFolder = string.Empty;
        private bool _subfoldersIncluded;
        private int _progress;
        private int _totalFiles = 1;

        private AppError? _lastError;
        private OperationState _state = OperationState.Idle;
        private CancellationTokenSource? _cts;

        public bool IsCancelled => State == OperationState.Cancelled;
        public bool HasError => LastError != null;

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
                if (!string.IsNullOrEmpty(_selectedFolder) 
                    && State == OperationState.Completed)
                {
                    LoadFileCommand.Execute(null);
                }
            }
        }
        public OperationState State
        {
            get => _state;
            private set
            {
                if (_state == value)
                {
                    return;
                }

                _state = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsCancelled));

                LoadFileCommand.RaiseCanExecuteChanged();
                ((RelayCommand)ExportFileCommand).RaiseCanExecuteChanged();
                ((RelayCommand)CancelCommand).RaiseCanExecuteChanged();
            }
        }
        public AppError? LastError
        {
            get => _lastError;
            set
            {
                _lastError = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasError));
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

        public ObservableCollection<TextFileItem> Files { get; } = new ObservableCollection<TextFileItem>();

        public ICommand SelectFolderCommand { get; }
        public RelayCommandAsync LoadFileCommand { get; }
        public ICommand ExportFileCommand { get; }
        public ICommand CancelCommand { get; }

        public MainViewModel(IFolderDialogService folderDialogService, IFileSystemService fileSystemService, IFileExportService fileExportService, IErrorMapper errorMapper)
        {
            _folderDialogService = folderDialogService;
            _fileSystemService = fileSystemService;
            _fileExportService = fileExportService;
            _errorMapper = errorMapper;

            SelectFolderCommand = new RelayCommand(SelectFolder);
            LoadFileCommand = new RelayCommandAsync(LoadFiles, CanLoadFiles);
            ExportFileCommand = new RelayCommand(ExportFile, CanExportFiles);
            CancelCommand = new RelayCommand(Cancel, () => State == OperationState.Loading);
        }

        private void SelectFolder()
        {
            SelectedFolder = _folderDialogService.LoadFolder();
        }
        private void Cancel()
        {
            if (State == OperationState.Loading)
            {
                State = OperationState.Cancelling;
            }
            _cts?.Cancel();
        }

        private async Task LoadFiles()
        {
            Files.Clear();
            TotalFiles = await _fileSystemService.TotalFilesAsync(SelectedFolder, SubfoldersIncluded);
            Progress = 0;
            State = OperationState.Loading;

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
                State = OperationState.Completed;
            }
            catch (OperationCanceledException)
            {
                State = OperationState.Cancelled;
                return;
            }
            catch (Exception ex)
            {
                State = OperationState.Failed;
                MessageBox.Show(ex.Message, "Error");
                return;
            }
            finally
            {
                if (State != OperationState.Cancelled 
                    && State != OperationState.Failed 
                    && State != OperationState.Completed)
                {
                    State = OperationState.Idle;
                }
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
                    MessageBox.Show($"An error occurred during export: {ex.Message}",
                        "Export Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return;
                }

                MessageBox.Show($"Files exported successfully.\nFiles: {Progress}\nSaved to: {path}",
                    "Export Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }

        private bool CanLoadFiles()
        {
            return !string.IsNullOrEmpty(SelectedFolder) 
                && Directory.Exists(SelectedFolder) 
                && (State == OperationState.Idle || State == OperationState.Completed || State == OperationState.Cancelled);
        }
        private bool CanExportFiles()
        {
            return State == OperationState.Completed && Files.Count > 0;
        }
    }
}
