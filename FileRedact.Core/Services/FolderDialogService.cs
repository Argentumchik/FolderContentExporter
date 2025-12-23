using FileRedact.Core.Interfaces;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace FileRedact.Core.Services
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
