using FolderContentExporter.Dto;
using FolderContentExporter.ViewModels;
using System.Windows;

namespace FolderContentExporter.View
{
    /// <summary>
    /// Логика взаимодействия для ExportDialogWindow.xaml
    /// </summary>
    public partial class ExportDialogWindow : Window
    {
        private readonly ExportDialogViewModel _vm;
        public ExportDialogWindow(ExportDialogViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            _vm = vm;

            Loaded += (_, _) =>
            {
                vm.CloseRequested += result => DialogResult = result;
            };
        }

        public ExportOptionsDto? GetExportData()
        {
            return _vm.ExportOptions;
        }
    }
}
