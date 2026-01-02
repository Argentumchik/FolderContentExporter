using FolderContentExporter.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FolderContentExporter.View
{
    /// <summary>
    /// Логика взаимодействия для ExportDialogWindow.xaml
    /// </summary>
    public partial class ExportDialogWindow : Window
    {
        public ExportDialogWindow()
        {
            InitializeComponent();

            Loaded += (_, _) =>
            {
                if (DataContext is ExportDialogViewModel vm)
                {
                    vm.CloseRequested += result => DialogResult = result;
                }
            };
        }
    }
}
