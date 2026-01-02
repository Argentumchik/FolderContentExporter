using FolderContentExporter.Interfaces;
using Microsoft.Win32;

namespace FolderContentExporter.Services
{
    public class FolderDialogService : IFolderDialogService
    {
        public string LoadFolder()
        {
            var dialog = new OpenFolderDialog();

            if (dialog.ShowDialog() == true)
            {
                return dialog.FolderName;
            }
            else
                return null;
        }
    }
}
