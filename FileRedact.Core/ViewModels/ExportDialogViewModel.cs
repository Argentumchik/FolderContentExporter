using FolderContentExporter.Commands;
using FolderContentExporter.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace FolderContentExporter.ViewModels
{
    public class ExportDialogViewModel : ViewModelBase
    {
        public event Action<bool?>? CloseRequested;

        private bool _includeFileName;
        private bool _includePath;
        private bool _includeExtension;
        private bool _includeSize;
        private bool _includeCreatedDate;
        private bool _includeModifiedDate;

        private string _exportFileName = "file";

        private ExportMode _selectedExportMode;

        public Array ExportModes => Enum.GetValues<ExportMode>();
        public ICommand ExitCommand { get; }

        public ExportDialogViewModel()
        {
            ExitCommand = new RelayCommand(Exit);
        }

        public bool IncludeFileName
        {
            get => _includeFileName;
            set
            {
                _includeFileName = value;
                OnPropertyChanged();
            }
        }
        public bool IncludePath
        {
            get => _includePath;
            set
            {
                _includePath = value;
                OnPropertyChanged();
            }
        }
        public bool IncludeExtension
        {
            get => _includeExtension;
            set
            {
                _includeExtension = value;
                OnPropertyChanged();
            }
        }
        public bool IncludeSize
        {
            get => _includeSize;
            set
            {
                _includeSize = value;
                OnPropertyChanged();
            }
        }
        public bool IncludeCreatedDate
        {
            get => _includeCreatedDate;
            set
            {
                _includeCreatedDate = value;
                OnPropertyChanged();
            }
        }
        public bool IncludeModifiedDate
        {
            get => _includeModifiedDate;
            set
            {
                _includeModifiedDate = value;
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
        public ExportMode SelectedExportMode
        {
            get => _selectedExportMode;
            set
            {
                _selectedExportMode = value;
                OnPropertyChanged();
            }
        }

        public ExportOptionsDto BuildOptions()
        {
            ExportFileName = string.Concat(ExportFileName.Split(Path.GetInvalidFileNameChars()));
            if (string.IsNullOrEmpty(ExportFileName))
            {
                MessageBox.Show("Please enter a valid export file name.", "Invalid File Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            var fields = ExportFields.None;

            if (IncludeFileName) fields |= ExportFields.Name;
            if (IncludeSize) fields |= ExportFields.Size;
            if (IncludeExtension) fields |= ExportFields.Extension;
            if (IncludePath) fields |= ExportFields.Path;
            if (IncludeCreatedDate) fields |= ExportFields.CreatedDate;
            if (IncludeModifiedDate) fields |= ExportFields.ModifiedDate;

            return new ExportOptionsDto
            {
                Fields = fields,
                Mode = SelectedExportMode,
                FileName = ExportFileName
            };
        }

        public void Exit()
        {
            CloseRequested?.Invoke(true);
        }
    }
}
