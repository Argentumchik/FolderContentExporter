using FileRedact.Core.Commands;
using FileRedact.Core.Dto;
using FileRedact.Core.Interfaces;
using FileRedact.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace FileRedact.Core.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IFolderDialogService _folderDialogService;
        private readonly IFileSystemService _fileSystemService;
        private readonly IFileExportService _fileExportService;

        private string _selectedFolder = string.Empty;
        private string _exportFileName = "file";
        private bool _subfoldersIncluded;
        private ExportMode _selectedExportMode;

        public Array ExportModes => Enum.GetValues<ExportMode>();

        public string SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                _selectedFolder = value;
                OnPropertyChanged();
            }
        }
        public bool SubfoldersIncluded 
        { 
            get => _subfoldersIncluded;
            set
            {
                _subfoldersIncluded = value;
                OnPropertyChanged();
                if (_selectedFolder != null)
                {
                    LoadFiles();
                }
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

        public ObservableCollection<TextFileItem> Files { get; } = [];

        public ICommand SelectFolderCommand { get; }
        public ICommand LoadFileCommand { get; }
        public ICommand ExportFileCommand { get; }

        public MainViewModel(IFolderDialogService folderDialogService, IFileSystemService fileSystemService, IFileExportService fileExportService)
        {
            _folderDialogService = folderDialogService;
            _fileSystemService = fileSystemService;
            _fileExportService = fileExportService;

            SelectFolderCommand = new RelayCommand(SelectFolder);
            LoadFileCommand = new RelayCommand(LoadFiles, CanLoadFiles);
            ExportFileCommand = new RelayCommand(ExportFile, CanExportFiles);
        }

        private void SelectFolder()
        {
            SelectedFolder = _folderDialogService.LoadFolder();
        }

        private void LoadFiles()
        {
            Files.Clear();

            var files = _fileSystemService.GetFiles(SelectedFolder, SubfoldersIncluded);

            foreach (var file in files)
            {
                Files.Add(file);
            }
        }

        private void ExportFile()
        {
            ExportFileName = string.Concat(ExportFileName.Split(Path.GetInvalidFileNameChars()));

            var path = _folderDialogService.LoadFolder();

            if (string.IsNullOrEmpty(ExportFileName))
            {
                MessageBox.Show("Please enter a valid export file name.", "Invalid File Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                if (SelectedExportMode == ExportMode.TXT)
                    _fileExportService.ExportToTxt(Files, path, ExportFileName);
                else if (SelectedExportMode == ExportMode.CSV)
                    _fileExportService.ExportToCsv(Files, path, ExportFileName);
                else if (SelectedExportMode == ExportMode.JSON)
                    _fileExportService.ExportToJson(Files, path, ExportFileName);
            } catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during export: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("Files exported successfully.", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
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
